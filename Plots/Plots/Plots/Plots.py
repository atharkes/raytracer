import matplotlib.pyplot as plt
import numpy as np
import math



distribution_start = 0.0
distribution_end = 1.0
steps = 1000

graph_start = distribution_start - 0.1
graph_end = distribution_end + 0.1

size = distribution_end - distribution_start
probability_density = 1 / size
step_size = size / steps

distances = np.arange(graph_start, graph_end, step_size)
material_densities = []
probability_densities = []
cummulative_probabilities = []
for distance in distances:
    if distribution_start <= distance <= distribution_end:
        if (distribution_end - distance) == 0:
            material_densities.append(float('inf'))
        else:
            material_densities.append(1 / (distribution_end - distance))
        probability_densities.append(probability_density)
    else:
        material_densities.append(0)
        probability_densities.append(0)
    cummulative_probabilities.append(min(max(distance - distribution_start, 0), size) * probability_density)


fig1, (material_density_plot, probability_density_plot, cummulative_probability_plot) = plt.subplots(3, 1, sharex=True)

material_density_plot.grid(axis='y', color='lightgray', linestyle='--')
material_density_plot.fill_between(distances, material_densities, 0, facecolor='lightblue')
material_density_plot.plot(distances, material_densities, linewidth=3)
material_density_plot.set_yscale('log')
material_density_plot.set_ylim([probability_density / 10, max(material_densities)])
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
cummulative_probability_plot.set_xlim([graph_start, graph_end])
cummulative_probability_plot.set_xlabel('distance')


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