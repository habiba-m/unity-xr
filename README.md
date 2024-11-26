# File Importer

This branch contains the `file-importer` module designed to import and visualize CSV data as point clouds in Unity. The code provides functionality to handle large datasets, convert them into meshes, and render them with materials for further use in 3D applications.

## Features

1. **Point Cloud Importation**:
   - Reads CSV files containing 3D point data.
   - Supports visualizing large datasets as point clouds.
   - Efficiently splits large datasets into manageable meshes.

2. **File Management**:
   - Allows selection of multiple CSV files via Unity's file explorer.
   - Logs errors and warnings for debugging file-related issues.

3. **Mesh Creation**:
   - Generates Unity meshes with vertices, colors, and indices.
   - Automatically assigns materials to meshes.

## Main Scripts

1. **`XyzRgbMeshImporter`**
   - Imports and processes CSV data as meshes.
   - Handles large datasets by creating child objects for sub-meshes.

2. **`DataImporter`**
   - Parses CSV files and maps scalar values to colors for visualization.
   - Creates and renders a point cloud object with the parsed data.

3. **`FileManager`**
   - Manages file selection and importing operations.
   - Interfaces with `PointCloudImporter` to process selected files.

## Usage

1. Attach the `FileManager` script to a Unity GameObject.
2. Use the `OpenExplorer` function to select CSV files.
3. The selected files will be imported and visualized as point clouds in the scene.

## File Format

- The CSV files should contain rows with 3D point data.
- Example structure:
  ```csv
  scalar1,scalar2,x,y,z
  0.5,1.0,12.3,45.6,78.9
  0.7,0.8,23.4,56.7,89.0
  ```

## Dependencies

- Unity Engine (tested with recent versions).
- `PointCloud/HexagonOpaque` shader (ensure it's available in the project).
