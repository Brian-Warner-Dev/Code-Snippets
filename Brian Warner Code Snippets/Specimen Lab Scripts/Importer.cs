using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//extracts the model and all the data needed for a specimen. 
//The old specimen is cleared out once a new specimen is loaded to keep memory usage down

public class Importer : MonoBehaviour {
	public Controller master;
	string _baseurl = "";
	string status;
	public Title title;
	Texture test;
	public Material opaque;
	public Material transparent;
	public bool useIP = true;
	Shader VertexLit;
	Shader TransparentVertexLit;

	public class Mat_Container
	{
		public int oindex;
		public int matindex;
		public string shadertype;
		public Color color;

		public Mat_Container()
		{
			oindex = 0;
			matindex = 0;
			shadertype = "Opaque";
			color = Color.white;
		}
	}

	public class Info_Container
	{
		public string part;
		public string system;
		public Texture icon;
		public List<int> oindex;
		public string description;
		public Info_Container()
		{
			part = "";
			oindex = new List<int>();
			system = "";
			description = "";
		}
		public void display()
		{
			string s = "Part: " + part + "\n";
			s += "System: " + system + "\n";
			for (int i = 0; i < oindex.Count; i++)
			{
				s += "Object: " + oindex[i].ToString() + "\n";
			}
			s += "Description: " + description;

			//print (s);
		}
	}

	void Start()
	{
		VertexLit = Shader.Find ("VertexLit");
		TransparentVertexLit = Shader.Find ("Transparent/VertexLit");
		_baseurl = Application.streamingAssetsPath + "/";
		if (Application.isEditor || Application.platform == RuntimePlatform.WindowsPlayer)
		{
			_baseurl = "https://kyrodo.us/S E Lab Web2/StreamingAssets/";
		}
		else
		{
			useIP = false;
		}
	}
	// Update is called once per frame
	void Update () {
		if (master.state != master.pstate && master.state == 1)
		{
			master.pstate = master.state;
			StartCoroutine (_Import ());
		}
	}

	void OnGUI()
	{
		GUI.skin.label.fontSize = Mathf.RoundToInt(Screen.height*.03f);
		GUILayout.Label (status,GUI.skin.label);
	}

	IEnumerator _Import()
	{
		List<List<Info_Container>> bodyinfo = null;
		master.sviews = null;
		string path = new Uri (_baseurl + master.specimenNames [title.loadindex] + "/" + master.specimenNames [title.loadindex] + ".obj").AbsoluteUri;
		if(useIP)
			path = "45.79.72.138" + path.Substring (22);
		ObjReader1.ObjData objdata = new ObjReader1.ObjData ();

		objdata = ObjReader1.use.ConvertFileAsync (path, true,opaque);
		while (!objdata.isDone)
		{
			upStatus (objdata);
			yield return null;
		}
		upStatus (objdata);
		yield return null;
		if (objdata.gameObjects == null)
		{
			status = master.specimenNames [title.loadindex] + ".obj or the mtl was not found in the " + master.specimenNames[title.loadindex] + " folder";
			yield break;
		}
		if (objdata.gameObjects.Length<=0)
		{
			status = master.specimenNames [title.loadindex] + ".obj or the mtl was not found in the " + master.specimenNames[title.loadindex] + " folder";
			yield break;
		}
		for (int i = 0; i < objdata.gameObjects.Length; i++)
		{
			Material[] cm = objdata.gameObjects [i].GetComponent<Renderer> ().materials;
			for (int j = 0; j < cm.Length; j++)
			{
				if (cm [j].shader == VertexLit)
				{
					Material temp = new Material (opaque);
					temp.mainTexture = cm [j].mainTexture;
					temp.color = cm [j].color;
					temp.name = cm [j].name;
					cm [j] = temp;
				}
				else if (cm [j].shader == TransparentVertexLit)
				{
					Material temp = new Material (transparent);
					temp.mainTexture = cm [j].mainTexture;
					temp.color = cm [j].color;
					temp.name = cm [j].name;
					cm [j] = temp;
				}
			}
			objdata.gameObjects [i].GetComponent<Renderer> ().materials = cm;
		}

		status = "Parenting meshes to empty root object.";
		yield return null;
		master.model = new GameObject ();
		master.model.transform.position = new Vector3 (0, -.8f, 0);
		master.model.name = master.specimenNames [title.loadindex];
		GameObject[] pivots = new GameObject[objdata.gameObjects.Length];
		for (int i = 0; i < objdata.gameObjects.Length; i++)
		{
			objdata.gameObjects [i].transform.parent = master.model.transform;
			objdata.gameObjects [i].transform.name = i.ToString ();
			pivots [i] = new GameObject ();
			pivots [i].transform.position = objdata.gameObjects [i].transform.position;
			pivots [i].transform.rotation = objdata.gameObjects [i].transform.rotation;
			pivots [i].transform.parent = objdata.gameObjects [i].transform;
			pivots [i].name = "Pivot";
		}

		//now search for name list
		path = new Uri(_baseurl + master.specimenNames [title.loadindex] + "/Name%20List.txt").AbsoluteUri;
		if(useIP)
			path = "45.79.72.138" + path.Substring (22);
		WWW www = new WWW (path);
		yield return www;
		if (www.error != null)
		{
			status = "Name List.txt not found in " + master.specimenNames [title.loadindex] + " folder. Files are case and space sensitive";
			yield break;
		}
		string[] lines = getlines(www);
		status = "Setting pivots.";
		yield return null;
		int s = 0;
		Vector3 v = new Vector3 (0, 0, 0);
		int oindex = 0;
		for (int i = 0; i < lines.Length; i++)
		{
			if (IsNullOrWhiteSpace (lines [i]))
				continue;
			if (s == 0)
			{
				//
			}
			if (s == 1)
			{
				if (lines [i].Length <= 3)
				{
					status = "float.parse(Substring(3)) error in Name List.txt at line " + lines [i];
					yield break;
				}
				if(!float.TryParse(lines[i].Substring(3), out v.x))
				{
					status = "float.parse(Substring(3)) error in Name List.txt at line " + lines [i];
					yield break;
				}
			}
			else if (s == 2)
			{
				if(lines[i].Length<=3)
				{
					status = "float.parse(Substring(3)) error in Name List.txt at line " + lines [i];
					yield break;
				}
				if(!float.TryParse(lines[i].Substring(3), out v.y))
				{
					status = "float.parse(Substring(3)) error in Name List.txt at line " + lines [i];
					yield break;
				}
			}
			else if (s == 3)
			{
				if(lines[i].Length<=3)
				{
					status = "float.parse(Substring(3)) error in Name List.txt at line " + lines [i];
					yield break;
				}
				if(!float.TryParse(lines[i].Substring(3), out v.z))
				{
					status = "float.parse(Substring(3)) error in Name List.txt at line " + lines [i];
					yield break;
				}
				if (oindex >= pivots.Length)
					break;
				pivots [oindex].transform.position = v;
			}
			s++;
			if (s > 3)
			{
				oindex++;
				s = 0;
			}
			yield return null;
		}

		path = new Uri (_baseurl+master.specimenNames[title.loadindex]+"/getDirectoryList.php").AbsoluteUri;
		if(useIP)
			path = "45.79.72.138" + path.Substring (22);
		www = new WWW (path);
		yield return www;
		if (www.error != null)
		{
			status = "getDirectoryList.php not found in " + master.specimenNames [title.loadindex] + " folder";
			yield break;
		}
		lines = getfolders (www);
		int iterator = 0;
		int views = 0;
		List<string> ls = new List<string> ();
		for (int i = 0; i < lines.Length; i++)
		{
			ls.Add (lines [i]);
		}
		for (int i = 0; i < ls.Count; i++)
		{
			if (ls.Contains (iterator.ToString ()))
			{
				views++;
				iterator++;
			}
		}
		bodyinfo = new List<List<Info_Container>>();
		master.allparts = new List<Part>();

		MatFlasher[] mf = new MatFlasher[objdata.gameObjects.Length];
		master.maxViews = views;
		for (int j = 0; j < objdata.gameObjects.Length; j++)
		{
			//j is object index
			mf[j] = objdata.gameObjects[j].AddComponent<MatFlasher>();
			mf[j].master = master;
			mf[j].wildcard = master.mempty;
			mf [j].marray = new List<Material[]> ();
			for (int k = 0; k < views; k++)
			{
				Material[] mtemp = new Material[objdata.gameObjects [j].GetComponent<Renderer> ().materials.Length + 1];
				for (int l = 0; l < objdata.gameObjects [j].GetComponent<Renderer> ().materials.Length; l++)
				{
					mtemp [l] = new Material(objdata.gameObjects [j].GetComponent<Renderer> ().materials [l]);
				}
				mtemp [mtemp.Length - 1] = mf [j].wildcard;
				mf [j].marray.Add (mtemp);
				if(k==0)
					objdata.gameObjects [j].GetComponent<Renderer> ().materials = mtemp;

				yield return null;
			}
		}

		for (int i = 0; i < views; i++)
		{
			string temppath = _baseurl + master.specimenNames [title.loadindex] + "/" + i.ToString () + "/";
			path = new Uri(temppath + "Info.txt").AbsoluteUri;
			if(useIP)
				path = "45.79.72.138" + path.Substring (22);
			www = new WWW (path);
			yield return www;
			if (www.error != null)
			{
				status = "Info.txt missing from " + master.specimenNames [title.loadindex] + "'s " + i.ToString () + " folder";
				yield break;
			}
			lines = getlines (www);
			//retrieve body part info 
			bodyinfo.Add(new List<Info_Container>());
			s = 0;
			iterator = 0;


			for (int j = 0; j < lines.Length; j++)
			{
				//i is view#
				//j is line#
				if (IsNullOrWhiteSpace (lines [j]))
					continue;
				if (s == 0)
				{
					status = "Loading body part info: " + "view " + i.ToString () + " body part# " + iterator.ToString ();
					yield return null;
					//print ("view " + i.ToString () + " container " + iterator.ToString () + " bodyinfo.count " + bodyinfo.Count);
					bodyinfo [i].Add (new Info_Container ());
					if (lines [j].Length <= 6)
					{
						status = "Error at line " + j.ToString () + " of Info.txt for view " + i.ToString () + " of specimen " + master.specimenNames [title.loadindex];
						status += "\n" + lines [j];
						yield break;
					}
					bodyinfo [i] [iterator].part = lines [j].Substring (6);
					path = new Uri (temppath + bodyinfo [i] [iterator].part + ".png").AbsoluteUri;
					if(useIP)
						path = "45.79.72.138" + path.Substring (22);
					www = new WWW (path);
					yield return www;
					if (www.error != null)
					{
						status = bodyinfo [i] [iterator].part + ".png missing from view folder " + i.ToString ();
						yield break;
					}
					bodyinfo [i] [iterator].icon = www.texture;
				}
				else if (s == 1)
				{
					if (lines [j].Length <= 8)
					{
						status = "Error at line " + j.ToString () + " of Info.txt for view " + i.ToString () + " of specimen " + master.specimenNames [title.loadindex];
						status += "\n" + lines [j];
						yield break;
					}
					bodyinfo [i] [iterator].system = lines [j].Substring (8);
				}
				else if (s == 2)
				{
					if (lines [j].Length <= 8)
					{
						status = "Error at line " + j.ToString () + " of Info.txt for view " + i.ToString () + " of specimen " + master.specimenNames [title.loadindex];
						status += "\n" + lines [j];
						yield break;
					}
					if (lines [j].Contains ("Object: "))
					{
						int num = 0;
						if(!Int32.TryParse(lines[j].Substring(8),out num))
						{
							status = "Error at line " + j.ToString () + " of Info.txt for view " + i.ToString () + " of specimen " + master.specimenNames [title.loadindex] + ": Older format, use object index instead of object name.";
							status += "\n" + lines [j];
							yield break;
						}
						bodyinfo [i] [iterator].oindex.Add (num);
					}
					else
						s++;
				}
				if (s == 3)
				{
					if (lines [j].Length <= 13)
					{
						status = "Error at line " + j.ToString () + " of Info.txt for view " + i.ToString () + " of specimen " + master.specimenNames [title.loadindex];
						yield break;
					}
					bodyinfo [i] [iterator].description = lines [j].Substring (13);
					for (int k = 0; k < bodyinfo [i] [iterator].oindex.Count; k++)
					{
						//k is oindex[k]
						if (k == 0)
						{
							Part _part = objdata.gameObjects [bodyinfo [i] [iterator].oindex [k]].AddComponent<Part> ();
							_part.mc = objdata.gameObjects [bodyinfo [i] [iterator].oindex [k]].AddComponent<MeshCollider> ();
							_part.pname = bodyinfo [i] [iterator].part;
							string[] syslist = getsystems (bodyinfo [i] [iterator].system);
							_part.systems = syslist;
							_part.description = bodyinfo [i] [iterator].description;
							_part.icon = bodyinfo [i] [iterator].icon;
							_part.type = i;
							_part.pivot = pivots [bodyinfo [i] [iterator].oindex [k]];
							_part.master = master;
							if (master.slist == null)
							{
								master.slist = new List<string> ();
							}
							for (int d = 0; d < _part.systems.Length; d++)
							{
								if (!master.slist.Contains (_part.systems[d]))
								{
									master.slist.Add (_part.systems[d]);
								}
							}

							master.allparts.Add (_part);
						}
						else
						{
							Paired pair = objdata.gameObjects [bodyinfo [i] [iterator].oindex [k]].AddComponent<Paired> ();
							pair.mc = objdata.gameObjects [bodyinfo [i] [iterator].oindex [k]].AddComponent<MeshCollider> ();
							pair.pairedPart = objdata.gameObjects [bodyinfo [i] [iterator].oindex [0]].GetComponent<Part> ();
						}
					}
					//bodyinfo [i] [iterator].display ();
					iterator++;
					yield return null;
				}
				if(s!=2)
					s++;
				if (s >= 4)
				{
					s = 0;
				}
				yield return null;
			} // end body part info loop
			status = "Loading Mat Edit data";
			yield return null;
			path = new Uri (temppath + "Mat Edit.txt").AbsoluteUri;
			if(useIP)
				path = "45.79.72.138" + path.Substring (22);
			www = new WWW (path);
			yield return www;
			if (www.error != null)
			{
				status = "Mat Edit.txt not found in " + master.specimenNames [title.loadindex] + "'s view " + i.ToString () + " folder";
				yield break;
			}
			lines = getlines (www);
			s = 0;
			iterator = 0;
			Mat_Container mc = new Mat_Container ();
			for(int j = 0; j<lines.Length; j++)
			{
				//j is lines iterator
				if (IsNullOrWhiteSpace (lines [j]))
					continue;
				if (s == 0)
				{
					mc = new Mat_Container ();
					int num = 0;
					if (lines [j].Length <= 8)
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
					if (Int32.TryParse (lines [j].Substring(8), out num))
					{
						mc.oindex = num;
					}
					else
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
				}
				else if (s == 1)
				{
					int num = 0;
					if (lines [j].Length <= 16)
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
					if (Int32.TryParse (lines [j].Substring(16), out num))
					{
						mc.matindex = num;
					}
					else
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
				}
				else if (s == 2)
				{
					if (lines [j].Length <= 13)
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
					if (lines [j].Substring(13) == "Opaque" || lines [j].Substring(13) == "Transparent")
					{
						mc.shadertype = lines [j].Substring (13);
					}
					else
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
				}
				else if (s == 3)
				{
					float value = 0;
					if (lines [j].Length <= 5)
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
					if (float.TryParse (lines [j].Substring (5), out value))
					{
						mc.color.r = value;
					}
					else
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
				}
				else if (s == 4)
				{
					float value = 0;
					if (lines [j].Length <= 7)
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
					if (float.TryParse (lines [j].Substring (7), out value))
					{
						mc.color.g = value;
					}
					else
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
				}
				else if (s == 5)
				{
					float value = 0;
					if (lines [j].Length <= 6)
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
					if (float.TryParse (lines [j].Substring (6), out value))
					{
						mc.color.b = value;
					}
					else
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
				}
				else if (s == 6)
				{
					float value = 0;
					if (lines [j].Length <= 7)
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
					if (float.TryParse (lines [j].Substring (7), out value))
					{
						mc.color.a = value;
					}
					else
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
					Material mtemp = new Material (opaque);
					if (mc.shadertype == "Transparent")
						mtemp = new Material (transparent);
					mtemp.color = mc.color;
					mtemp.mainTexture = mf [mc.oindex].marray [i] [mc.matindex].mainTexture;
					if(mc.oindex>= mf.Length)
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
					if (mc.matindex >= mf [mc.oindex].marray [i].Length - 1)
					{
						status = "Error at line " + j.ToString () + " in Mat Edit.txt of view " + i.ToString ();
						yield break;
					}
					mf [mc.oindex].marray [i] [mc.matindex] = mtemp;
				}
				s++;
				if (s > 6)
					s = 0;
				yield return null;
			}

			path = new Uri(temppath + "ViewName.txt").AbsoluteUri;
			if(useIP)
				path = "45.79.72.138" + path.Substring (22);
			//print (path);
			www = new WWW (path);
			yield return www;
			if (www.error != null)
			{
				status = "ViewName.txt missing from view " + i.ToString () + " of " + master.specimenNames [title.loadindex];
				yield break;
			}
			if (master.sviews == null)
			{
				master.sviews = new string[1];
				master.sviews [0] = www.text;
			}
			else
			{
				string[] stemp = new string[master.sviews.Length + 1];
				for (int x = 0; x < master.sviews.Length; x++)
				{
					stemp [x] = master.sviews [x];
				}
				stemp [stemp.Length - 1] = www.text;
				master.sviews = stemp;
			}
			yield return null;
		} //end view folders loop

		master.objects = objdata.gameObjects;
		master.state++;
		yield break;
	}

	void upStatus(WWW www)
	{
		status = (www.progress*100).ToString ("0")+"%";
	}
	void upStatus(ObjReader1.ObjData oj)
	{
		status = (oj.progress*100).ToString ("0")+"%";
	}

	string[] getlines(WWW www)
	{
		char[] eliminate = new char[]{ '\n', '\r' };
		string splitText = www.text;
		return splitText.Split (eliminate, System.StringSplitOptions.RemoveEmptyEntries);
	}
	string[] getsystems(string s)
	{
		char[] eliminate = new char[]{ ',' };
		string[] result = s.Split (eliminate, System.StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < result.Length; i++)
		{
			result [i] = result [i].Trim ();
		}
		return result;
	}

	public bool IsNullOrWhiteSpace(string value)
	{
		if (value != null)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsWhiteSpace(value[i]))
				{
					return false;
				}
			}
		}
		return true;
	}

	string[] getfolders(WWW www)
	{
		char[] eliminator = { ',' };
		return www.text.Split (eliminator);
	}
	
}
