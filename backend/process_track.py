'''The code defines functions to interpolate coordinates of inner and outer boundaries of a track using
    various methods and provides a GUI for selecting input files and displaying the interpolated
    boundaries for comparison.
    
    Parameters
    ----------
    file_path
    `file_path` is a string parameter representing the path to a file that contains coordinates data.
    
    Returns
    -------
    The code provided is a Python script that creates a GUI application using Tkinter for interpolating
    and visualizing track boundaries from input coordinate files.
    
'''
    
import os
import numpy as np
import matplotlib.pyplot as plt
import tkinter as tk

from scipy.interpolate import splprep, splev, interp1d, CubicSpline, RBFInterpolator
from tkinter import filedialog, messagebox

def load_coordinates(file_path):
    if not os.path.exists(file_path):
        messagebox.showerror("❌ ERROR", f"File {file_path} does not exists!")
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
                print(f"⚠️  Line was ignored: {line}")

    coordinates = np.array(coordinates)
    print(f"📊 {len(coordinates)} points were read from {file_path}")
    return coordinates

def interpolate_spline(coords, num_points=10000):
    if len(coords) == 0:
        return np.array([])
    
    tck, u = splprep([coords[:, 0], coords[:, 1]], s=0)
    u_new = np.linspace(0, 1, num_points)
    return np.array(splev(u_new, tck)).T

def interpolate_linear(coords, num_points=10000):
    if len(coords) == 0:
        return np.array([])
    
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

def choose_files():
    file_paths = filedialog.askopenfilenames(title="Choose files with data", filetypes=[("Text files", "*.txt")])
    if file_paths:
        entry_inner_path.delete(0, tk.END)
        entry_inner_path.insert(0, file_paths[0])
        entry_outer_path.delete(0, tk.END)
        entry_outer_path.insert(0, file_paths[1])
    else:
        messagebox.showwarning("ATTENTION", "Choose exactly two files (outer and inner boundary of the track)!")
        
def process_files():
    inner_path = entry_inner_path.get()
    outer_path = entry_outer_path.get()
    
    if not inner_path or not outer_path:
        messagebox.showwarning("ATTENTION", "File not chosen!")
        return 
    
    inner_coords = load_coordinates(inner_path)
    outer_coords = load_coordinates(outer_path)
    
    if len(inner_coords) == 0 or len(outer_coords) == 0:
        return

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

        if len(inner_smooth) == 0 or len(outer_smooth) == 0:
                continue
            
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
    
root = tk.Tk()
root.title("Interpolation of the track")

frame = tk.Frame(root)
frame.pack(pady=20)

entry_inner_path = tk.Entry(frame, width=50)
entry_inner_path.pack(side=tk.LEFT, padx=5)
entry_outer_path = tk.Entry(frame, width=50)
entry_outer_path.pack(side=tk.LEFT, padx=5)

btn_choose_files = tk.Button(frame, text="Choose files", command=choose_files)
btn_choose_files.pack(side=tk.LEFT)

btn_process = tk.Button(root, text="Interpolate", command=process_files)
btn_process.pack(pady=10)

root.mainloop()