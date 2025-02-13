import os
import numpy as np
import matplotlib.pyplot as plt
from scipy.interpolate import splprep, splev, interp1d, CubicSpline, RBFInterpolator

def load_coordinates(file_path):
    if not os.path.exists(file_path):
        print(f"❌ BŁĄD: Plik {file_path} nie istnieje!")
        return np.array([]) 

    with open(file_path, "r", encoding="utf-8") as file:
        data = file.read().strip().split()  
        
    coordinates = []
    for line in data:
        parts = line.strip().split(',')
        if len(parts) == 3 and all(parts):  
            try:
                coordinates.append(tuple(map(float, parts)))
            except ValueError:
                print(f"⚠️  Ignorowana błędna linia: {line}")

    coordinates = np.array(coordinates)
    print(f"📊 Wczytano {len(coordinates)} punktów z {file_path}")
    return coordinates

inner_coords = load_coordinates(r"..\data\Monza\MonzaInnerBoundary.txt")
outer_coords = load_coordinates(r"..\data\Monza\MonzaOuterBoundary.txt")

# Sprawdzenie danych
if len(inner_coords) == 0 or len(outer_coords) == 0:
    print("❌ Brak danych, sprawdź pliki wejściowe!")
    exit()

def interpolate_spline(coords, num_points=10000):
    tck, u = splprep([coords[:, 0], coords[:, 1]], s=0)
    u_new = np.linspace(0, 1, num_points)
    return np.array(splev(u_new, tck)).T

def interpolate_linear(coords, num_points=10000):
    u = np.linspace(0, 1, len(coords))
    u_new = np.linspace(0, 1, num_points)
    interp_x = interp1d(u, coords[:, 0], kind='linear')
    interp_y = interp1d(u, coords[:, 1], kind='linear')
    return np.column_stack((interp_x(u_new), interp_y(u_new)))

def interpolate_cubic(coords, num_points=10000):
    u = np.linspace(0, 1, len(coords))
    u_new = np.linspace(0, 1, num_points)
    interp_x = CubicSpline(u, coords[:, 0])
    interp_y = CubicSpline(u, coords[:, 1])
    return np.column_stack((interp_x(u_new), interp_y(u_new)))

def interpolate_rbf(coords, num_points=10000):
    u = np.linspace(0, 1, len(coords)).reshape(-1, 1)
    u_new = np.linspace(0, 1, num_points).reshape(-1, 1)
    interp_x = RBFInterpolator(u, coords[:, 0])
    interp_y = RBFInterpolator(u, coords[:, 1])
    return np.column_stack((interp_x(u_new), interp_y(u_new)))

methods = {
    "B-spline": interpolate_spline,
    "Linear": interpolate_linear,
    "Cubic Spline": interpolate_cubic,
    "RBF": interpolate_rbf
}

fig, axes = plt.subplots(2, 2, figsize=(12, 10))

for ax, (method_name, method_func) in zip(axes.flat, methods.items()):
    inner_smooth = method_func(inner_coords)
    outer_smooth = method_func(outer_coords)

    ax.plot(inner_smooth[:, 0], inner_smooth[:, 1], 'r-', label="Inner Boundary")
    ax.plot(outer_smooth[:, 0], outer_smooth[:, 1], 'b-', label="Outer Boundary")
    
    ax.set_title(f"Interpolation: {method_name}")
    ax.set_xlabel("Longitude")
    ax.set_ylabel("Latitude")
    ax.set_aspect('equal', adjustable='datalim')
    ax.legend()
    ax.grid()

plt.suptitle("Comparison of Different Interpolation Methods")
plt.tight_layout()
plt.show()
