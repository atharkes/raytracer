import argparse
import math
import json
import numpy as np
import matplotlib.pyplot as plt

# Define command line options (also generates --help and error handling)
CLI = argparse.ArgumentParser()

# Add and parse arguments
CLI.add_argument('--data_path', type=str)
CLI.add_argument('--distances', nargs="*", type=float, default=[])
CLI.add_argument('--material_densities', nargs="*", type=float, default=[])
CLI.add_argument('--probability_densities', nargs="*", type=float, default=[])
CLI.add_argument('--cummulative_probabilities', nargs="*", type=float, default=[])

args = CLI.parse_args()

def plot_distribution_seperate(distances, material_densities, probability_densities, cummulative_probabilities):
    steps = len(distances)

    fig1, (material_density_plot, probability_density_plot, cummulative_probability_plot) = plt.subplots(3, 1, sharex=True)

    material_density_plot.grid(axis='y', color='lightgray', linestyle='--')
    material_density_plot.fill_between(distances, material_densities, 0, facecolor='lightblue')
    material_density_plot.plot(distances, material_densities, linewidth=3)
    material_density_plot.set_yscale('log')
    material_density_plot.set_ylim([0.1, 2000])
    material_density_plot.set_ylabel('material density')

    probability_density_plot.grid(axis='y', color='lightgray', linestyle='--')
    probability_density_plot.fill_between(distances, probability_densities, 0, facecolor='lightcoral', edgecolor='firebrick', hatch='///')
    probability_density_plot.plot(distances, probability_densities, linewidth=3, color='crimson')
    probability_density_plot.set_ylim([0, 4.2])
    probability_density_plot.set_ylabel('probability density')

    cummulative_probability_plot.grid(axis='y', color='lightgray', linestyle='--')
    cummulative_probability_plot.fill_between(distances, cummulative_probabilities, 0, facecolor='plum', edgecolor='purple', hatch='///')
    cummulative_probability_plot.plot(distances, cummulative_probabilities, linewidth=3, color='darkviolet')
    cummulative_probability_plot.set_ylim([0, 1.05])
    cummulative_probability_plot.set_ylabel('cumulative probability')
    cummulative_probability_plot.set_xlim([distances[0], distances[-1]])
    cummulative_probability_plot.set_xlabel('distance')

    plt.show()

def plot_distribution_combined(distances, material_densities, probability_densities, cummulative_probabilities):
    fig2, (combinedPlot) = plt.subplots(1, 1, sharex=True)

    combinedPlot.plot(distances, material_densities, c='red', label='Material Density', linewidth=3)
    combinedPlot.fill_between(distances, material_densities, 0, facecolor='lightcoral')
    combinedPlot.plot(distances, probability_densities, c='blue', label='Probability Density', linewidth=1)
    combinedPlot.fill_between(distances, probability_densities, 0, facecolor='none', edgecolor='blue', hatch='///')
    combinedPlot.legend(loc='upper left')
    combinedPlot.set_yscale('log')
    combinedPlot.set_ylim([0.0001, max(material_densities)])
    combinedPlot.set_xlabel('distance')
    combinedPlot.set_ylabel('density')

    plt.show()


distances = args.distances
material_densities = args.material_densities
probability_densities = args.probability_densities
cummulative_probabilities = args.cummulative_probabilities

if (args.data_path != ''):
    with open(args.data_path, 'r') as f:
        data = json.load(f)
        for distribution_data in data.values():
            print(distribution_data)
            distances = distribution_data['Distances']
            material_densities = distribution_data['MaterialDensities']
            probability_densities = distribution_data['ProbabilityDensities']
            cummulative_probabilities = distribution_data['CummulativeProbabilities']
            plot_distribution_seperate(distances, material_densities, probability_densities, cummulative_probabilities)