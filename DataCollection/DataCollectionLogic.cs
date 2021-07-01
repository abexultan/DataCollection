using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ColossalFramework.IO;

namespace DataCollection
{
	public class DataCollectionLogic : MonoBehaviour
	{
		public string camposfile = @"PUT LOCATION (FULLPATH) OF campos.txt HERE";
		public CameraController controller;
		public Camera camera;
		string FullSessionDirectoryName;
		public int counter = 0;
		public int FrameExportIndex;
		public TextWriter tw_camera;
		public List<Vector3> pos_arr;
		public List<Vector2> rot_arr;
		public List<float> height_arr;
		public List<float> size_arr;
        private Utils.Tuple<List<Vector3>, List<Vector2>, List<float>, List<float>> coords;

		void Start()
		{
			controller = FindObjectOfType<CameraController>();
			camera = controller.GetComponent<Camera>();
			FrameExportIndex = 0;
		}



		void Awake()
		{
			if (!Directory.Exists(DataLocation.localApplicationData + @"\ModConfig\"))
				Directory.CreateDirectory(DataLocation.localApplicationData + @"\ModConfig\");
			if (!Directory.Exists(DataLocation.localApplicationData + @"\ModConfig\DataCollection\"))
				Directory.CreateDirectory(DataLocation.localApplicationData + @"\ModConfig\DataCollection\");
			if (!Directory.Exists(DataLocation.localApplicationData + @"\ModConfig\DataCollection\"))
				Directory.CreateDirectory(DataLocation.localApplicationData + @"\ModConfig\DataCollection\");

			string sessionDirectoryName = "GameSession-" + DateTime.Now.ToString("yyyyMMddHHmmss");
			FullSessionDirectoryName = DataLocation.localApplicationData + @"\ModConfig\DataCollection\" + sessionDirectoryName + @"\";
			if (!Directory.Exists(FullSessionDirectoryName))
				Directory.CreateDirectory(FullSessionDirectoryName);
			if (!Directory.Exists(FullSessionDirectoryName + @"\images\"))
                Directory.CreateDirectory(FullSessionDirectoryName + @"\images\");

			if (!Directory.Exists(FullSessionDirectoryName + @"\labels\"))
				Directory.CreateDirectory(FullSessionDirectoryName + @"\labels\");

			tw_camera = new StreamWriter(FullSessionDirectoryName + "campos" + ".txt");
            try
            {
				coords = Utils.getCoords(camposfile);
				pos_arr = coords.Item1;
				rot_arr = coords.Item2;
				height_arr = coords.Item3;
				size_arr = coords.Item4;
			}
            catch (Exception e)
            {
				Log.Message(e.ToString());
            }
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Delete))
			{
				tw_camera.WriteLine($"{controller.m_currentPosition} {controller.m_currentAngle} {controller.m_currentHeight} {controller.m_currentSize}");
			}

			if (Input.GetKeyDown(KeyCode.Backspace))
            {
				if (counter == pos_arr.Count)
                {
					counter = 0;
                }
				Vector3 camPos = pos_arr[counter];
				Vector2 camRot = rot_arr[counter];
				float camHeight = height_arr[counter];
				float camSize = size_arr[counter];

				controller.m_targetAngle = camRot;
				controller.m_targetPosition = camPos;
				controller.m_targetHeight = camHeight;
				controller.m_targetSize = camSize;

				counter++;
            }

			if (Input.GetKeyDown(KeyCode.L))
			{
				TextWriter tw = new StreamWriter(FullSessionDirectoryName + @"labels\Frame_" + FrameExportIndex.ToString() + ".txt");
				List<double> center_x_list = new List<double>();
				List<double> center_y_list = new List<double>();
				List<float> z_distance_list = new List<float>();
				List<int> cls_idx_list = new List<int>();
				Vehicle[] vehicles = VehicleManager.instance.m_vehicles.m_buffer;
				foreach (Vehicle v in vehicles)
				{
					try
					{
						Vector3 worldPos = v.GetSmoothPosition(v.m_infoIndex);
						Vector3 screenPos = camera.WorldToScreenPoint(worldPos);

						if (screenPos.x < 0.0f || screenPos.x > Screen.width || screenPos.y < 0.0f || screenPos.y > Screen.height || screenPos.z < 0.0f || screenPos.z > 350f)
						{
							continue;
						}
						else
						{
							Ray ray = new Ray(camera.transform.position, worldPos - camera.transform.position);
							ToolBase.RaycastInput raycastInput = new ToolBase.RaycastInput(ray, screenPos.z + 50f);
							raycastInput.m_ignoreBuildingFlags = Building.Flags.None;
							raycastInput.m_ignoreCitizenFlags = CitizenInstance.Flags.Deleted | CitizenInstance.Flags.Underground;
							raycastInput.m_ignoreNodeFlags = NetNode.Flags.Underground | NetNode.Flags.Deleted;
							raycastInput.m_ignoreSegmentFlags = NetSegment.Flags.Deleted;
							raycastInput.m_ignoreTreeFlags = TreeInstance.Flags.Deleted | TreeInstance.Flags.Hidden;
							raycastInput.m_ignorePropFlags = PropInstance.Flags.Hidden | PropInstance.Flags.Deleted;
							raycastInput.m_ignoreParkedVehicleFlags = VehicleParked.Flags.Deleted;
							raycastInput.m_ignoreVehicleFlags = Vehicle.Flags.Underground | Vehicle.Flags.Deleted;
							bool res = Utils.RayCast(raycastInput, out ToolBase.RaycastOutput output);
							if (output.m_vehicle > 0)
							{
								int cls_idx = 0;
								if (vehicles[output.m_vehicle].Info.name.ToLower().Contains("truck"))
								{
									cls_idx = 1;
								}
								else if (vehicles[output.m_vehicle].Info.name.ToLower().Contains("scooter"))
								{
									cls_idx = 3;
								}
								double center_x = Math.Round(screenPos.x / Screen.width, 5);
								double center_y = Math.Round((Screen.height - screenPos.y) / Screen.height, 5);
								center_x_list.Add(center_x);
								center_y_list.Add(center_y);
								z_distance_list.Add(screenPos.z);
								cls_idx_list.Add(cls_idx);

							}
						}
					}
					catch (Exception e)
					{
						Log.Message(e.ToString());

					}
				}

				VehicleParked[] vehicles_parked = VehicleManager.instance.m_parkedVehicles.m_buffer;
				foreach (VehicleParked v_parked in vehicles_parked)
				{
					try
					{
						Vector3 worldPos = v_parked.m_position;
						Vector3 screenPos = camera.WorldToScreenPoint(worldPos);

						if (screenPos.x < 0.0f || screenPos.x > Screen.width || screenPos.y < 0.0f || screenPos.y > Screen.height || screenPos.z < 0.0f || screenPos.z > 350f)
						{
							continue;
						}
						else
						{
							Ray ray = new Ray(camera.transform.position, worldPos - camera.transform.position);
							ToolBase.RaycastInput raycastInput = new ToolBase.RaycastInput(ray, screenPos.z + 50f);
							raycastInput.m_ignoreBuildingFlags = Building.Flags.None;
							raycastInput.m_ignoreCitizenFlags = CitizenInstance.Flags.Deleted | CitizenInstance.Flags.Underground;
							raycastInput.m_ignoreNodeFlags = NetNode.Flags.Underground | NetNode.Flags.Deleted;
							raycastInput.m_ignoreSegmentFlags = NetSegment.Flags.Deleted;
							raycastInput.m_ignoreTreeFlags = TreeInstance.Flags.Deleted | TreeInstance.Flags.Hidden;
							raycastInput.m_ignorePropFlags = PropInstance.Flags.Hidden | PropInstance.Flags.Deleted;
							raycastInput.m_ignoreParkedVehicleFlags = VehicleParked.Flags.Deleted;
							raycastInput.m_ignoreVehicleFlags = Vehicle.Flags.Underground | Vehicle.Flags.Deleted;
							bool res = Utils.RayCast(raycastInput, out ToolBase.RaycastOutput output);
							if (output.m_parkedVehicle > 0)
							{
								int cls_idx = 0;
								if (vehicles_parked[output.m_parkedVehicle].Info.name.ToLower().Contains("truck"))
                                {
									cls_idx = 1;
                                } else if (vehicles_parked[output.m_parkedVehicle].Info.name.ToLower().Contains("scooter"))
                                {
									cls_idx = 3;
                                }
								double center_x = Math.Round(screenPos.x / Screen.width, 5);
								double center_y = Math.Round((Screen.height - screenPos.y) / Screen.height, 5);
								center_x_list.Add(center_x);
								center_y_list.Add(center_y);
								z_distance_list.Add(screenPos.z);
								cls_idx_list.Add(cls_idx);

							}
						}
					}
					catch (Exception e)
					{
						Log.Message(e.ToString());

					}
				}
				CitizenInstance[] citizens = CitizenManager.instance.m_instances.m_buffer;
				foreach (CitizenInstance c in citizens)
				{
					try
					{
						Vector3 worldPos = c.GetSmoothPosition(c.m_infoIndex);
						Vector3 screenPos = camera.WorldToScreenPoint(worldPos);

						if (screenPos.x < 0.0f || screenPos.x > Screen.width || screenPos.y < 0.0f || screenPos.y > Screen.height || screenPos.z < 0.0f || screenPos.z > 350f)
						{
							continue;
						}
						else
						{
							Ray ray = new Ray(camera.transform.position, worldPos - camera.transform.position);
							ToolBase.RaycastInput raycastInput = new ToolBase.RaycastInput(ray, screenPos.z + 50f);
							raycastInput.m_ignoreBuildingFlags = Building.Flags.None;
							raycastInput.m_ignoreCitizenFlags = CitizenInstance.Flags.Deleted | CitizenInstance.Flags.Underground;
							raycastInput.m_ignoreNodeFlags = NetNode.Flags.Underground | NetNode.Flags.Deleted;
							raycastInput.m_ignoreSegmentFlags = NetSegment.Flags.Deleted;
							raycastInput.m_ignoreTreeFlags = TreeInstance.Flags.Deleted | TreeInstance.Flags.Hidden;
							raycastInput.m_ignorePropFlags = PropInstance.Flags.Hidden | PropInstance.Flags.Deleted;
							raycastInput.m_ignoreParkedVehicleFlags = VehicleParked.Flags.Deleted;
							raycastInput.m_ignoreVehicleFlags = Vehicle.Flags.Underground | Vehicle.Flags.Deleted;
							
							bool res = Utils.RayCast(raycastInput, out ToolBase.RaycastOutput output);
							if (output.m_citizenInstance > 0)
							{
								int cls_idx = 2;
								double center_x = Math.Round(screenPos.x / Screen.width, 5);
								double center_y = Math.Round((Screen.height - screenPos.y) / Screen.height, 5);
								center_x_list.Add(center_x);
								center_y_list.Add(center_y);
								z_distance_list.Add(screenPos.z);
								cls_idx_list.Add(cls_idx);
							}
						}
					}
					catch (Exception e)
					{
						Log.Message(e.ToString());
					}
				}


				float z_sum = 0;
				foreach(float z_val in z_distance_list)
                {
					z_sum += z_val;
                }
				float z_average = z_sum / z_distance_list.Count;

				for (int i = 0; i < center_x_list.Count; i++)
                {
					float bbox_size = 15 * (z_average / z_distance_list[i]);
					float width = (bbox_size * 2) / Screen.width;
					float height = (bbox_size * 2) / Screen.height;
					tw.WriteLine($"{cls_idx_list[i]} {center_x_list[i]} {center_y_list[i]} {width} {height}");
                }

				Application.CaptureScreenshot($@"{FullSessionDirectoryName}images\Frame_{FrameExportIndex}.png");
				FrameExportIndex += 1;
				tw.Close();
			}
		}

		void OnDisable()
		{
			tw_camera.Close();
		}
	}
}
