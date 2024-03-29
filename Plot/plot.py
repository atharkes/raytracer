import argparse
import math
import json
import numpy as np
import matplotlib.pyplot as plt
from matplotlib.colors import LinearSegmentedColormap

# Define command line options (also generates --help and error handling)
CLI = argparse.ArgumentParser()

# Add and parse arguments
CLI.add_argument('--data_path', type=str)
CLI.add_argument('--distances', nargs="*", type=float, default=[])
CLI.add_argument('--material_densities', nargs="*", type=float, default=[])
CLI.add_argument('--probability_densities', nargs="*", type=float, default=[])
CLI.add_argument('--cumulative_probabilities', nargs="*", type=float, default=[])

args = CLI.parse_args()

def plot_distribution_seperate(distances, material_densities, probability_densities, cumulative_probabilities):
    steps = len(distances)

    fig, (material_density_plot, probability_density_plot, cummulative_probability_plot) = plt.subplots(3, 1, sharex=True, figsize=(6, 6.5))

    material_density_plot.set_ylabel('Material Density')
    material_density_plot.grid(axis='y', color='lightgray', linestyle='--')
    material_density_plot.fill_between(distances, material_densities, 0, facecolor='lightblue')
    material_density_plot.plot(distances, material_densities, linewidth=3)
    material_density_plot.set_yscale('log')
    material_density_plot.set_ylim([0.1, 2000])

    probability_density_plot.set_ylabel('Free-flight Distribution')
    probability_density_plot.grid(axis='y', color='lightgray', linestyle='--')
    probability_density_plot.fill_between(distances, probability_densities, 0, facecolor='lightcoral', edgecolor='firebrick', hatch='///')
    probability_density_plot.plot(distances, probability_densities, linewidth=3, color='crimson')
    probability_density_plot.set_ylim([0, 4.2])

    cummulative_probability_plot.set_ylabel('Absorption')
    cummulative_probability_plot.grid(axis='y', color='lightgray', linestyle='--')
    cummulative_probability_plot.fill_between(distances, cumulative_probabilities, 0, facecolor='plum', edgecolor='purple', hatch='///')
    cummulative_probability_plot.plot(distances, cumulative_probabilities, linewidth=3, color='darkviolet')
    cummulative_probability_plot.set_ylim([0, 1.05])

    cummulative_probability_plot.set_xlabel('Distance')
    cummulative_probability_plot.set_xlim([distances[0], distances[-1]])

    plt.show()

def plot_distributions_combined(data):
    fig, (material_density_plot, free_flight_plot, absorption_plot, transmittance_plot) = plt.subplots(4, 1, sharex=True, figsize=(6, 9))

    distances = data['Distances']

    material_density_plot.set_ylabel('Material Density')
    material_density_plot.grid(axis='y', color='lightgray', linestyle='--')
    material_density_plot.set_yscale('log')
    material_density_plot.set_ylim([0.1, 2000])

    free_flight_plot.set_ylabel('Free-flight Distribution\n')
    free_flight_plot.grid(axis='y', color='lightgray', linestyle='--')
    free_flight_plot.set_ylim([0, 4.2])

    absorption_plot.set_ylabel('Absorption')
    absorption_plot.grid(axis='y', color='lightgray', linestyle='--')
    absorption_plot.set_ylim([0, 1.05])
       
    transmittance_plot.set_ylabel('Transmittance')
    transmittance_plot.grid(axis='y', color='lightgray', linestyle='--')
    transmittance_plot.set_ylim([0, 1.05])

    transmittance_plot.set_xlabel('Distance')
    transmittance_plot.set_xlim([distances[0], distances[-1]])

    sample_count = len(distances)
    previous_material_densities = [0] * sample_count
    previous_free_flight_probabilities = [0] * sample_count
    previous_absorption_factors = [0] * sample_count
    previous_transmittance_factors = [1] * sample_count

    accumulated_material_densities = [0] * sample_count
    accumulated_free_flight_probabilities = [0] * sample_count
    accumulated_absorption_factors = [0] * sample_count
    accumulated_transmittance_factors = [1] * sample_count

    line_width = 3
    line_style = (0, (5, 0))
    hatch_style = '..'
    color_count = len(data['Distributions']) + 2
    cmap = plt.cm.get_cmap('hsv', color_count)
    def line_color(i):
        color = cmap(i)
        factor = 0.7
        alpha = 0.8
        return (factor * color[0], factor * color[1], factor * color[2], alpha)
    def face_color(i):
        color = cmap(i)
        factor = 0.9
        alpha = 1.0
        return (factor * color[0], factor * color[1], factor * color[2], alpha)
    def edge_color(i):
        color = cmap(i)
        factor = 0.5
        alpha = 0.5
        return (factor * color[0], factor * color[1], factor * color[2], alpha)

    index = 1
    for distribution_data in data['Distributions'].values():
        material_densities = distribution_data['MaterialDensities']
        free_flight_probabilities = distribution_data['ProbabilityDensities']
        absorption_factors = distribution_data['CumulativeProbabilities']
        transmittance_factors = list(map(lambda x: 1 - x, absorption_factors))

        accumulated_material_densities = np.add(accumulated_material_densities, material_densities) 
        accumulated_free_flight_probabilities = np.add(accumulated_free_flight_probabilities, free_flight_probabilities)
        accumulated_absorption_factors = list(map(lambda x, y: 1 - (1 - x) * (1 - y), accumulated_absorption_factors, absorption_factors))
        accumulated_transmittance_factors = list(map(lambda x, y: x * y, accumulated_transmittance_factors, transmittance_factors))

        material_density_plot.fill_between(distances, accumulated_material_densities, previous_material_densities, facecolor=face_color(index), zorder=10+index)
        #material_density_plot.plot(distances, material_densities, linewidth=line_width, color=line_color(index), linestyle=line_style, zorder=20-index)
        
        free_flight_plot.fill_between(distances, accumulated_free_flight_probabilities, previous_free_flight_probabilities, facecolor=face_color(index), zorder=10-index)
        #free_flight_plot.plot(distances, free_flight_probabilities, linewidth=line_width, color=line_color(index), linestyle=line_style, zorder=20-index)
        
        absorption_plot.fill_between(distances, accumulated_absorption_factors, previous_absorption_factors, facecolor=face_color(index), zorder=10-index)
        #absorption_plot.plot(distances, absorption_factors, linewidth=line_width, color=line_color(index), linestyle=line_style, zorder=20-index)

        previous_material_densities = accumulated_material_densities
        previous_free_flight_probabilities = accumulated_free_flight_probabilities
        previous_absorption_factors = accumulated_absorption_factors
        previous_transmittance_factors = accumulated_transmittance_factors
        index = index + 1

    combined_index = 0
    material_density_plot.plot(distances, accumulated_material_densities, linewidth=line_width, color=line_color(combined_index), linestyle=line_style, zorder=20)
    free_flight_plot.plot(distances, accumulated_free_flight_probabilities, linewidth=line_width, color=line_color(combined_index), linestyle=line_style, zorder=20)
    absorption_plot.plot(distances, accumulated_absorption_factors, linewidth=line_width, color=line_color(combined_index), linestyle=line_style, zorder=20)

    transmittance_plot.fill_between(distances, accumulated_transmittance_factors, 0, facecolor=face_color(combined_index), zorder=10)
    transmittance_plot.plot(distances, accumulated_transmittance_factors, linewidth=line_width, color=line_color(combined_index), linestyle=line_style, zorder=20)
    plt.show()


distances = args.distances
material_densities = args.material_densities
probability_densities = args.probability_densities
cumulative_probabilities = args.cumulative_probabilities

if (args.data_path != ''):
    with open(args.data_path, 'r') as f:
        data = json.load(f)
        plot_distributions_combined(data)