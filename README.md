# Mapping the Railway formalism onto different domains

For more information about this research project, read the [report](report/report.pdf).

### Directory Structure

- `Railway`: the Railway formalism modelled in AToMPM
- `TrainSchedule`: the Train Schedule formalism modelled in AToMPM
- `railway_wd`: working directory for the Railway formalism, used to store temporary files etc. (place this folder in AToMPM's root directory)
    - `RailwayDEVS`: the railway model in DEVS
    - `RailwayUnity`: the railway model in Unity
- `report`: the sources of the report and the report itself

### Dependencies

- LoLA: [https://www2.informatik.hu-berlin.de/top/lola/loladoku/index.html](https://www2.informatik.hu-berlin.de/top/lola/loladoku/index.html)
- PythonPDEVS: [http://msdl.cs.mcgill.ca/projects/DEVS/PythonPDEVS](http://msdl.cs.mcgill.ca/projects/DEVS/PythonPDEVS)
- Unity: [https://unity.com/](https://unity.com/)
    - ~~Railway assets: [https://assetstore.unity.com/packages/3d/vehicles/land/simple-trains-cartoon-assets-86794](https://assetstore.unity.com/packages/3d/vehicles/land/simple-trains-cartoon-assets-86794) (import the `.unitypackage` package in the `Railway/Assets/Resources` directory in Unity)~~