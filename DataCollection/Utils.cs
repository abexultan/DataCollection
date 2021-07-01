using System;
using System.Collections.Generic;
using ColossalFramework.Math;
using ColossalFramework;
using UnityEngine;


namespace DataCollection
{
    class Utils
    {

		public class Tuple<T, U, V, X>
		{
			public T Item1 { get; private set; }
			public U Item2 { get; private set; }
			public V Item3 { get; private set; }
			public X Item4 { get; private set; }
			public Tuple(T item1, U item2, V item3, X item4)
			{
				Item1 = item1;
				Item2 = item2;
				Item3 = item3;
				Item4 = item4;
			}
		}

		public static Tuple<List<Vector3>, List<Vector2>, List<float>, List<float>> getCoords(string path)
		{
			string[] text = System.IO.File.ReadAllLines(path);
			List<Vector2> rot_arr = new List<Vector2>();
			List<Vector3> p_arr = new List<Vector3>();
			List<float> size_arr = new List<float>();
			List<float> height_arr = new List<float>();
			foreach (string line in text)
			{

				string[] splitline = line.Split('(', ')');

				string[] pos_string = splitline[1].Split(',');
				string[] rot_string = splitline[3].Split(',');
				string height_str = splitline[4].Split(' ')[1];
				string size_str = splitline[4].Split(' ')[2];

				float[] pos = new float[3];
				for (int j = 0; j < 3; j++)
				{
					pos[j] = float.Parse(pos_string[j]);
				}

				float[] rot = new float[2];
				for (int j = 0; j < 2; j++)
				{
					rot[j] = float.Parse(rot_string[j]);
				}

				float height = float.Parse(height_str);
				float size = float.Parse(size_str);

				p_arr.Add(new Vector3(pos[0], pos[1], pos[2]));
				rot_arr.Add(new Vector2(rot[0], rot[1]));
				size_arr.Add(size);
				height_arr.Add(height);

			}
			return new Tuple<List<Vector3>, List<Vector2>, List<float>, List<float>>(p_arr, rot_arr, height_arr, size_arr);
		}

		public static int getRandomCarId()
        {
            int i = 0;
            Vehicle[] vehiclesBuffer = Singleton<VehicleManager>.instance.m_vehicles.m_buffer;
            for (int p = 0; p < vehiclesBuffer.Length; p++)
            {
                try
                {
                    Vehicle v = vehiclesBuffer[p];
                    Vehicle.Frame v_frame = v.GetLastFrameData();
                    Vector3 pos = v.GetSmoothPosition((ushort)p);
                    if (pos != new Vector3(0.0f, 0.0f, 0.0f) && !v_frame.m_underground && !v_frame.m_insideBuilding)
                    {
                        i = p;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Log.Message(e.ToString());
                }
            }
            return i;
        }

		public Vehicle.Flags GetVehicleFlags()
		{
			if ((Singleton<ToolManager>.instance.m_properties.m_mode & ItemClass.Availability.Game) == 0)
			{
				return Vehicle.Flags.LeftHandDrive | Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding;
			}
			if (Singleton<InfoManager>.instance.CurrentMode == InfoManager.InfoMode.Transport || Singleton<InfoManager>.instance.CurrentMode == InfoManager.InfoMode.Traffic || Singleton<InfoManager>.instance.CurrentMode == InfoManager.InfoMode.EscapeRoutes || Singleton<InfoManager>.instance.CurrentMode == InfoManager.InfoMode.Tours)
			{
				return (Vehicle.Flags)0;
			}
			if (Singleton<InfoManager>.instance.CurrentMode == InfoManager.InfoMode.TrafficRoutes && Singleton<InfoManager>.instance.CurrentSubMode != 0)
			{
				return Vehicle.Flags.LeftHandDrive | Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding;
			}
			return Vehicle.Flags.Underground;
		}

		public static bool RayCast(ToolBase.RaycastInput input, out ToolBase.RaycastOutput output)
		{
			Vector3 origin = input.m_ray.origin;
			Vector3 normalized = input.m_ray.direction.normalized;
			Vector3 vector = input.m_ray.origin + normalized * input.m_length;
			Segment3 ray = new Segment3(origin, vector);
			output.m_hitPos = vector;
			output.m_overlayButtonIndex = 0;
			output.m_netNode = 0;
			output.m_netSegment = 0;
			output.m_building = 0;
			output.m_propInstance = 0;
			output.m_treeInstance = 0u;
			output.m_vehicle = 0;
			output.m_parkedVehicle = 0;
			output.m_citizenInstance = 0;
			output.m_transportLine = 0;
			output.m_transportStopIndex = 0;
			output.m_transportSegmentIndex = 0;
			output.m_district = 0;
			output.m_park = 0;
			output.m_disaster = 0;
			output.m_currentEditObject = false;
			bool result = false;
			float num = input.m_length;
			if (!input.m_ignoreTerrain && Singleton<TerrainManager>.instance.RayCast(ray, out var hit))
			{
				float num2 = Vector3.Distance(hit, origin) + 100f;
				if (num2 < num)
				{
					output.m_hitPos = hit;
					result = true;
					num = num2;
				}
			}
			if ((input.m_ignoreNodeFlags != NetNode.Flags.All || input.m_ignoreSegmentFlags != NetSegment.Flags.All) && Singleton<NetManager>.instance.RayCast(input.m_buildObject as NetInfo, ray, input.m_netSnap, input.m_segmentNameOnly, input.m_netService.m_service, input.m_netService2.m_service, input.m_netService.m_subService, input.m_netService2.m_subService, input.m_netService.m_itemLayers, input.m_netService2.m_itemLayers, input.m_ignoreNodeFlags, input.m_ignoreSegmentFlags, out hit, out output.m_netNode, out output.m_netSegment))
			{
				float num3 = Vector3.Distance(hit, origin);
				if (num3 < num)
				{
					output.m_hitPos = hit;
					result = true;
					num = num3;
				}
				else
				{
					output.m_netNode = 0;
					output.m_netSegment = 0;
				}
			}
			if (input.m_ignoreBuildingFlags != Building.Flags.All && Singleton<BuildingManager>.instance.RayCast(ray, input.m_buildingService.m_service, input.m_buildingService.m_subService, input.m_buildingService.m_itemLayers, input.m_ignoreBuildingFlags, out hit, out output.m_building))
			{
				float num4 = Vector3.Distance(hit, origin);
				if (num4 < num)
				{
					output.m_hitPos = hit;
					output.m_netNode = 0;
					output.m_netSegment = 0;
					result = true;
					num = num4;
				}
				else
				{
					output.m_building = 0;
				}
			}
			if (input.m_ignoreDisasterFlags != DisasterData.Flags.All && Singleton<DisasterManager>.instance.RayCast(ray, input.m_ignoreDisasterFlags, out hit, out output.m_disaster))
			{
				float num5 = Vector3.Distance(hit, origin);
				if (num5 < num)
				{
					output.m_hitPos = hit;
					output.m_netNode = 0;
					output.m_netSegment = 0;
					output.m_building = 0;
					result = true;
					num = num5;
				}
				else
				{
					output.m_disaster = 0;
				}
			}
			if (input.m_currentEditObject && Singleton<ToolManager>.instance.m_properties.RaycastEditObject(ray, out hit))
			{
				float num6 = Vector3.Distance(hit, origin);
				if (num6 < num)
				{
					output.m_hitPos = hit;
					output.m_netNode = 0;
					output.m_netSegment = 0;
					output.m_building = 0;
					output.m_disaster = 0;
					output.m_currentEditObject = true;
					result = true;
					num = num6;
				}
			}
			if (input.m_ignorePropFlags != PropInstance.Flags.All && Singleton<PropManager>.instance.RayCast(ray, input.m_propService.m_service, input.m_propService.m_subService, input.m_propService.m_itemLayers, input.m_ignorePropFlags, out hit, out output.m_propInstance))
			{
				float num7 = Vector3.Distance(hit, origin) - 0.5f;
				if (num7 < num)
				{
					output.m_hitPos = hit;
					output.m_netNode = 0;
					output.m_netSegment = 0;
					output.m_building = 0;
					output.m_disaster = 0;
					output.m_currentEditObject = false;
					result = true;
					num = num7;
				}
				else
				{
					output.m_propInstance = 0;
				}
			}
			if (input.m_ignoreTreeFlags != TreeInstance.Flags.All && Singleton<TreeManager>.instance.RayCast(ray, input.m_treeService.m_service, input.m_treeService.m_subService, input.m_treeService.m_itemLayers, input.m_ignoreTreeFlags, out hit, out output.m_treeInstance))
			{
				float num8 = Vector3.Distance(hit, origin) - 1f;
				if (num8 < num)
				{
					output.m_hitPos = hit;
					output.m_netNode = 0;
					output.m_netSegment = 0;
					output.m_building = 0;
					output.m_disaster = 0;
					output.m_propInstance = 0;
					output.m_currentEditObject = false;
					result = true;
					num = num8;
				}
				else
				{
					output.m_treeInstance = 0u;
				}
			}
			if (Singleton<VehicleManager>.instance.RayCast(ray, input.m_ignoreVehicleFlags, input.m_ignoreParkedVehicleFlags, out hit, out output.m_vehicle, out output.m_parkedVehicle))
			{
				float num9 = Vector3.Distance(hit, origin) - 0.5f;
				if (num9 < num)
				{
					output.m_hitPos = hit;
					output.m_netNode = 0;
					output.m_netSegment = 0;
					output.m_building = 0;
					output.m_disaster = 0;
					output.m_propInstance = 0;
					output.m_treeInstance = 0u;
					output.m_currentEditObject = false;
					result = true;
					num = num9;
				}
				else
				{
					output.m_vehicle = 0;
					output.m_parkedVehicle = 0;
				}
			}
			if (input.m_ignoreCitizenFlags != CitizenInstance.Flags.All && Singleton<CitizenManager>.instance.RayCast(ray, input.m_ignoreCitizenFlags, out hit, out output.m_citizenInstance))
			{
				float num10 = Vector3.Distance(hit, origin) - 0.5f;
				if (num10 < num)
				{
					output.m_hitPos = hit;
					output.m_netNode = 0;
					output.m_netSegment = 0;
					output.m_building = 0;
					output.m_disaster = 0;
					output.m_propInstance = 0;
					output.m_treeInstance = 0u;
					output.m_vehicle = 0;
					output.m_parkedVehicle = 0;
					output.m_currentEditObject = false;
					result = true;
					num = num10;
				}
				else
				{
					output.m_citizenInstance = 0;
				}
			}
			if (input.m_ignoreTransportFlags != TransportLine.Flags.All && Singleton<TransportManager>.instance.RayCast(input.m_ray, input.m_length, input.m_transportTypes, out hit, out output.m_transportLine, out output.m_transportStopIndex, out output.m_transportSegmentIndex))
			{
				float num11 = Vector3.Distance(hit, origin) - 2f;
				if (num11 < num)
				{
					output.m_hitPos = hit;
					output.m_netNode = 0;
					output.m_netSegment = 0;
					output.m_building = 0;
					output.m_disaster = 0;
					output.m_propInstance = 0;
					output.m_treeInstance = 0u;
					output.m_vehicle = 0;
					output.m_parkedVehicle = 0;
					output.m_citizenInstance = 0;
					output.m_currentEditObject = false;
					result = true;
				}
				else
				{
					output.m_transportLine = 0;
				}
			}
			if (input.m_ignoreDistrictFlags != District.Flags.All || input.m_ignoreParkFlags != DistrictPark.Flags.All)
			{
				if (input.m_districtNameOnly)
				{
					if (Singleton<DistrictManager>.instance.RayCast(ray, input.m_rayRight, out hit, out output.m_district, out output.m_park))
					{
						output.m_hitPos = hit;
					}
				}
				else
				{
					if (input.m_ignoreDistrictFlags != District.Flags.All)
					{
						output.m_district = Singleton<DistrictManager>.instance.SampleDistrict(output.m_hitPos);
						if ((Singleton<DistrictManager>.instance.m_districts.m_buffer[output.m_district].m_flags & input.m_ignoreDistrictFlags) != 0)
						{
							output.m_district = 0;
						}
					}
					if (input.m_ignoreParkFlags != DistrictPark.Flags.All)
					{
						output.m_park = Singleton<DistrictManager>.instance.SamplePark(output.m_hitPos);
						if ((Singleton<DistrictManager>.instance.m_parks.m_buffer[output.m_park].m_flags & input.m_ignoreParkFlags) != 0)
						{
							output.m_park = 0;
						}
						if (output.m_park != 0)
						{
							output.m_district = 0;
						}
					}
				}
				if (output.m_district != 0 || output.m_park != 0)
				{
					output.m_netNode = 0;
					output.m_netSegment = 0;
					output.m_building = 0;
					output.m_disaster = 0;
					output.m_propInstance = 0;
					output.m_treeInstance = 0u;
					output.m_vehicle = 0;
					output.m_parkedVehicle = 0;
					output.m_citizenInstance = 0;
					output.m_transportLine = 0;
					output.m_transportStopIndex = 0;
					output.m_transportSegmentIndex = 0;
					output.m_currentEditObject = false;
					result = true;
				}
			}
			if (output.m_netNode != 0)
			{
				NetManager instance = Singleton<NetManager>.instance;
				NetInfo info = instance.m_nodes.m_buffer[output.m_netNode].Info;
				output.m_overlayButtonIndex = info.m_netAI.RayCastNodeButton(output.m_netNode, ref instance.m_nodes.m_buffer[output.m_netNode], ray);
			}
			return result;
		}
	}
}
