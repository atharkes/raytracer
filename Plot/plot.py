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
    fig, (material_density_plot, probability_density_plot, cummulative_probability_plot) = plt.subplots(3, 1, sharex=True, figsize=(6, 6.5))

    distances = data['Distances']

    material_density_plot.set_ylabel('Material Density')
    material_density_plot.grid(axis='y', color='lightgray', linestyle='--')
    material_density_plot.set_yscale('log')
    material_density_plot.set_ylim([0.1, 2000])

    probability_density_plot.set_ylabel('Free-flight Distribution')
    probability_density_plot.grid(axis='y', color='lightgray', linestyle='--')
    probability_density_plot.set_ylim([0, 4.2])

    cummulative_probability_plot.set_ylabel('Absorption')
    cummulative_probability_plot.grid(axis='y', color='lightgray', linestyle='--')
    cummulative_probability_plot.set_ylim([0, 1.05])

    cummulative_probability_plot.set_xlabel('Distance')
    cummulative_probability_plot.set_xlim([distances[0], distances[-1]])

    previous_material_densities = [0] * len(distances)
    previous_probability_densities = [0] * len(distances)
    previous_cumulative_probabilities = [0] * len(distances)

    cmap = plt.cm.get_cmap('hsv', len(data['Distributions']) + 1)
    c_index = 0
    
    for distribution_data in data['Distributions'].values():
        line_width = 3
        line_style = (0, (5, 0))
        color = cmap(c_index)
        line_color = (color[0], color[1], color[2], 0.8)
        face_color = (color[0], color[1], color[2], 0.5)
        edge_color = (color[0] * 0.5, color[1] * 0.5, color[2] * 0.5, 0.5)
        hatch_style = '..'

        material_densities = distribution_data['MaterialDensities']
        probability_densities = np.add(previous_probability_densities, distribution_data['ProbabilityDensities'])
        cumulative_probabilities = list(map(lambda x, y: 1 - (1 - x) * (1 - y), previous_cumulative_probabilities, distribution_data['CumulativeProbabilities']))

        material_density_plot.fill_between(distances, material_densities, 0, facecolor=face_color, zorder=10+c_index)
        material_density_plot.plot(distances, material_densities, linewidth=line_width, color=line_color, linestyle=line_style, zorder=20+c_index)
        
        probability_density_plot.fill_between(distances, probability_densities, previous_probability_densities, facecolor=face_color, zorder=10-c_index)
        probability_density_plot.plot(distances, probability_densities, linewidth=line_width, color=line_color, linestyle=line_style, zorder=20-c_index)
        
        cummulative_probability_plot.fill_between(distances, cumulative_probabilities, previous_cumulative_probabilities, facecolor=face_color, zorder=10-c_index)
        cummulative_probability_plot.plot(distances, cumulative_probabilities, linewidth=line_width, color=line_color, linestyle=line_style, zorder=20-c_index)

        previous_material_densities = material_densities
        previous_probability_densities = probability_densities
        previous_cumulative_probabilities = cumulative_probabilities
        c_index = c_index + 1
    plt.show()


distances = args.distances
material_densities = args.material_densities
probability_densities = args.probability_densities
cumulative_probabilities = args.cumulative_probabilities

if (args.data_path != ''):
    with open(args.data_path, 'r') as f:
        data = json.load(f)
        plot_distributions_combined(data)