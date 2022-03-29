import argparse
import math
import numpy as np
import matplotlib.pyplot as plt
import Distributions.uniform

# Define command line options (also generates --help and error handling)
CLI = argparse.ArgumentParser()

# Add arguments
CLI.add_argument("--distances", nargs="*", type=float, default=[])
CLI.add_argument("--material_densities", nargs="*", type=float, default=[])
CLI.add_argument("--probability_densities", nargs="*", type=float, default=[])
CLI.add_argument("--cummulative_probabilities", nargs="*", type=float, default=[])

# Parse the command line
args = CLI.parse_args()

# Access CLI options
print("distances: %r" % args.distances)
print("material_densities: %r" % args.material_densities)
print("probability_densities: %r" % args.probability_densities)
print("cummulative_probabilities: %r" % args.cummulative_probabilities)

def plot_distribution_seperate(distances, material_densities, probability_densities, cummulative_probabilities):
    steps = len(distances)

    fig1, (material_density_plot, probability_density_plot, cummulative_probability_plot) = plt.subplots(3, 1, sharex=True)

    material_density_plot.grid(axis='y', color='lightgray', linestyle='--')
    material_density_plot.fill_between(distances, material_densities, 0, facecolor='lightblue')
    material_density_plot.plot(distances, material_densities, linewidth=3)
    material_density_plot.set_yscale('log')

    material_density_plot.set_ylim([min(x for x in material_densities if x > 0) / 10, max(x for x in material_densities if x > 0)])
    material_density_plot.set_ylabel('material density')

    probability_density_plot.grid(axis='y', color='lightgray', linestyle='--')
    probability_density_plot.fill_between(distances, probability_densities, 0, facecolor='lightcoral', edgecolor='firebrick', hatch='///')
    probability_density_plot.plot(distances, probability_densities, linewidth=3, color='crimson')
    probability_density_plot.set_ylim([0, math.log(steps, 10) + 1])
    probability_density_plot.set_ylabel('probability density')

    cummulative_probability_plot.grid(axis='y', color='lightgray', linestyle='--')
    cummulative_probability_plot.fill_between(distances, cummulative_probabilities, 0, facecolor='plum', edgecolor='purple', hatch='///')
    cummulative_probability_plot.plot(distances, cummulative_probabilities, linewidth=3, color='darkviolet')
    cummulative_probability_plot.set_ylim([0, math.log(steps, 10) + 1])
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

#(distances, material_densities, probability_densities, cummulative_probabilities) = Distributions.uniform.get_data(0.0, 1.0)
plot_distribution_seperate(distances, material_densities, probability_densities, cummulative_probabilities)