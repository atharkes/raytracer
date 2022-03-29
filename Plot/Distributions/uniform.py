import matplotlib.pyplot as plt
import numpy as np
import math

def get_data(start, end):
    distribution_start = start
    distribution_end = end
    distribution_size = distribution_end - distribution_start
    probability_density = 1 / distribution_size

    steps = 1000
    step_size = distribution_size / steps

    graph_start = distribution_start - 0.1
    graph_end = distribution_end + 0.1
    
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
        cummulative_probabilities.append(min(max(distance - distribution_start, 0), distribution_size) * probability_density)
    
    return distances, material_densities, probability_densities, cummulative_probabilities