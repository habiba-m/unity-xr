# Leveraging Unity for Advanced Scientific Data Visualization and Interaction in Virtual Reality (LUADV – VR)
This Unity project visualizes point cloud datasets in a time-series format within a VR environment. Designed for use with the Meta Quest headset, it enables users to explore particle datasets interactively.

# Features
- Time-Series Playback: Visualize datasets one timestep at a time.
- CSV Data Integration: Automatically download and parse CSV files formatted as ssxyz (Scalar, Scalar, X-position, Y-position, Z-position).
- Interactive Controls: Playback controls (start, pause, next, back) allow navigation through timesteps.
- VR Movement: Explore datasets with joystick movement or teleportation.
- Headset Compatibility: Works seamlessly with Meta Quest using Quest Link.
# Setup Instructions
## Prerequisites
- Unity Editor (version 2020.3 or later recommended)
- Meta Quest headset with Quest Link enabled
- A server hosting .csv files formatted as ssxyz
### File Format
Ensure your CSV files follow this format:

**Scalar, Scalar, X-position, Y-position, Z-position**
# Steps
- Clone this repository to your local machine. (git clone <repository-url>)
- Open the project in Unity.
- Update the flaskServerURL in the CSVDownloader script with the URL of your server hosting the CSV files.
- Ensure your CSV files are accessible and correctly formatted as described above.
# How to Use
1. Launch the Program
- Start the Unity scene.
- The program will automatically download the CSV files and store them as point cloud objects in memory.
2. Start Quest Link
- Put on the Meta Quest headset.
- Enable Quest Link to connect to your PC.
3. Start Time-Series Playback
- Press the Start button to display the first timestep and begin the time series.
The system will automatically display subsequent timesteps with a delay.
4. Utilize Playback Controls
- Start: Begin the time series from the first timestep.
- Pause: Pause the current timestep.
- Continue: Resume playback from the current timestep.
- Next: Manually display the next timestep.
- Back: Manually display the previous timestep.
- Refresh: Reset to the first timestep.
5. Explore the Dataset
- Use joystick movement or teleportation in VR to navigate around the dataset and analyze the point cloud.
