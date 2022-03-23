import matplotlib.pyplot as plt
import numpy as np
import math

start = 0.0
end = 1.0
steps = 10000

size = end - start
probability_density = 1 / size
step = size / steps

distances = np.arange(start, end, step)
material_densities = 1 / (end - distances)
probability_densities = np.repeat(probability_density, steps)


fig1, (probability_plot, density_plot) = plt.subplots(2, 1, sharex=True)

density_plot.plot(distances, material_densities, linewidth=3)
density_plot.fill_between(distances, material_densities, 0, facecolor='lightblue')
density_plot.set_yscale('log')
density_plot.set_ylim([material_densities[0] / 10, material_densities[-2]])
density_plot.set_xlabel('distance')
density_plot.set_ylabel('material density')

probability_plot.plot(distances, probability_densities, linewidth=2, color='red')
probability_plot.fill_between(distances, probability_densities, 0, facecolor='lightcoral', edgecolor='indianred', hatch='////')
probability_plot.set_ylim([0, math.log(steps, 10)])
probability_plot.set_ylabel('probability density')


fig2, (combinedPlot) = plt.subplots(1, 1, sharex=True)

combinedPlot.plot(distances, material_densities, c='red', label='Material Density', linewidth=3)
combinedPlot.fill_between(distances, material_densities, 0, facecolor='lightcoral')
combinedPlot.plot(distances, probability_densities, c='blue', label='Probability Density', linewidth=1)
combinedPlot.fill_between(distances, probability_densities, 0, facecolor='none', edgecolor='blue', hatch='///')
combinedPlot.legend(loc='upper left')
combinedPlot.set_yscale('log')
combinedPlot.set_ylim([0.0001, material_densities[-2]])
combinedPlot.set_xlabel('distance')
combinedPlot.set_ylabel('density')

plt.show()