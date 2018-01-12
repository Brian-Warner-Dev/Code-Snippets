using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SFB;
using System.IO;
using System;

//specimen creator main code

public class V2Builder : MonoBehaviour {
	public GameObject master;
	List<Material> earlyMats = null;
	Color ccolor = new Color(1,1,1,1);
	public int mchoice;
	float scrollcooldown = 0;
	int ostartindex = 0;
	public int bchoice;
	public GizmoControllerCS GC;
	public int state=0;
	int pstate = 0;
	string sname = "";
	string[] path = null;
	string[] p_path = null;
	Texture icon = null;
	string _iconpath = "";
	string prefix = "file:///";
	string opath = "";
	GameObject model = null;
	public GameObject[] mesh = null;
	GameObject[] pivots = null;
	string status = "";
	bool loading = false;
	string mtlfilename = "";
	string mtl = "";
	string obj = "";
	List<Texture> otextures = new List<Texture> ();
	List<string> otxpaths = new List<string>();
	List<MFlash> lmfs = new List<MFlash>();
	public Material opaque;
	public Material transparent;
	public Texture etleft;
	public Texture etright;
	public Texture t_select;

	public Material selected;
	public Material phantom;
	public Material empty;
	List<string> _orinames = new List<string>();

	public int ochoice=-1;
	public float flash;
	int dir= 1;
	public List<ViewData> vdata = new List<ViewData> ();
	public int vchoice;
	GUIStyle centerStyle = new GUIStyle();
	bool firstFrame = true;

	public Shader VertexLit;
	public Shader TransparentVertexLit;

	public class ViewData : MonoBehaviour
	{
		public string viewname = "";
		public List<string> partnames = new List<string>();
		public List<List<int>> oindicesperpart = new List<List<int>> ();
		public List<string> descriptions = new List<string>();
		public List<string> systemnames = new List<string>();
		public List<Texture> particons = new List<Texture> ();
		public List<Material[]> vmats = new List<Material[]>();
		public ViewData()
		{
		}
		public void AddPart()
		{
			partnames.Add ("");
			oindicesperpart.Add (new List<int> ());
			descriptions.Add ("");
			systemnames.Add ("");
			particons.Add (new Texture ());
		}

		public void RemovePartAt(int index)
		{
			partnames.RemoveAt (index);
			oindicesperpart.RemoveAt (index);
			descriptions.RemoveAt (index);
			systemnames.RemoveAt (index);
			particons.RemoveAt (index);
		}
	}
	void Update()
	{
		if (GC == null)
		{
			GC = FindObjectOfType<GizmoControllerCS> ();
		}
		if (firstFrame)
		{
			Material[] m = FindObjectsOfType<Material> ();
			earlyMats = new List<Material> ();
			for (int i = 0; i < m.Length; i++)
			{
				earlyMats.Add (m [i]);
			}
			m = null;
			firstFrame = false;
		}
		if (scrollcooldown > 0)
		{
			scrollcooldown -= Time.deltaTime;
		}
		if (pstate != state)
		{
			if (state == 0)
			{

				Material[] m = FindObjectsOfType<Material> ();
				for (int i = 0; i < m.Length; i++)
				{
					if (!earlyMats.Contains (m [i]))
					{
						Destroy (m [i]);
						m [i] = null;
					}
				}

				Texture[] t = FindObjectsOfType <Texture>();
				for (int i = 0; i < t.Length; i++)
				{
					Destroy (t [i]);
					t [i] = null;
				}
				t = null;

				Texture2D[] t2 = FindObjectsOfType <Texture2D>();
				for (int i = 0; i < t2.Length; i++)
				{
					Destroy (t2 [i]);
					t2 [i] = null;
				}
				t2 = null;
				System.GC.Collect ();
				Resources.UnloadUnusedAssets();

				GameObject go = (GameObject) Instantiate (master);
				go.name = "Controller";
				go.SetActive (true);
				go.GetComponent<V2Builder> ().master = master;
				Destroy (gameObject);


				System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe")); //new program 
				Application.Quit(); //kill current process
			}
			if (state == 4||state==9)
			{
				ochoice = -1;
			}
			if (state == 6)
			{
				bchoice = 0;
			}
			if (state == 10)
			{
				status = "";
			}
			pstate = state;
		}
		flash += Time.deltaTime * dir * 3;
		if (flash <= 0)
		{
			dir = 1;
		}
		else if (flash >= 1)
		{
			dir = -1;
		}
		Color c = selected.color;
		c.a = flash;
		selected.color = c;
	}
	IEnumerator Start()
	{
		VertexLit = Shader.Find ("VertexLit");
		TransparentVertexLit = Shader.Find ("Transparent/VertexLit");
		centerStyle.alignment = TextAnchor.MiddleCenter;
		centerStyle.normal.textColor = Color.white;

		while (true)
		{
			if (state == 0)
			{
				if (path != p_path && path != null)
				{
					p_path = path;
					if (path.Length > 0)
					{
						state = 11;
					}
				}
			}
			if (state == 2)
			{
				if (path != p_path && path != null)
				{
					p_path = path;

					if (path.Length > 0)
					{
						WWW www = new WWW (prefix + path [0]);
						yield return www;
						if (www.error != null)
						{
							status = www.error;
						}
						else
						{
							status = "Load Successful";
						}
						icon = www.texture;
						_iconpath = path [0];
					}
				}
			}
			else if (state == 3)
			{
				if (path != p_path && path != null)
				{
					p_path = path;
					if (path.Length > 0)
					{
						//load obj as text file
						status = "Loading OBJ";
						yield return null;
						opath = path [0];
						WWW www = new WWW (prefix + path [0]);
						while (!www.isDone)
						{
							upStatus (www);
							yield return null;
						}
						if (www.error != null)
						{
							status = www.error;
							loading = false;
						}
						else
						{
							obj = www.text;
							string[] lines = getlines (www);
							_orinames = new List<string> ();
							for (int i = 0; i < lines.Length; i++)
							{
								if (lines [i].Contains ("mtllib"))
								{
									mtlfilename = lines [i].Substring (7);
								}

								if (lines [i].Contains ("usemtl"))
								{
									_orinames.Add (lines [i - 1].Substring (2));
								}
							}

							//load mtl and textures
							string basepath = opath.Substring (0, opath.Length - Path.GetFileName (opath).Length);
							string mtlpath = prefix + basepath + mtlfilename;
							www = new WWW (mtlpath);
							yield return www;
							if (www.error != null)
							{
								status = "Unable to find " + mtlpath;
								yield break;
							}
							mtl = www.text;
							lines = getlines (mtl);
							otextures = new List<Texture> ();
							otxpaths = new List<string> ();
							for (int i = 0; i < lines.Length; i++)
							{
								if (lines [i].Contains ("map_Kd"))
								{
									www = new WWW (prefix + basepath + lines [i].Substring (7));
									status = "Loading Textures";
									while (!www.isDone)
									{
										yield return null;
									}
									if (www.error != null)
									{
										status = "Unable to find " + prefix + basepath + lines [i].Substring (7) + "\nMake sure all texture files are in the same folder.";
										yield break;
									}
									if (!otxpaths.Contains (lines [i].Substring (7)))
									{
										otextures.Add (www.texture);
										otxpaths.Add (lines [i].Substring (7));
									}
								}
							}

							//load obj
							List<ViewData> vdata = new List<ViewData> ();
							ObjReader.ObjData odata = new ObjReader.ObjData ();
							lmfs = new List<MFlash> ();
							odata = ObjReader.use.ConvertFileAsync (prefix + opath, true, opaque);
							while (!odata.isDone)
							{
								upStatus (odata);
								status = "Loading Model " + status;
								yield return null;
							}
								

							mesh = odata.gameObjects;

							for (int i = 0; i < mesh.Length; i++)
							{
								Material[] cm = mesh [i].GetComponent<Renderer> ().materials;
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
								mesh [i].GetComponent<Renderer> ().materials = cm;
							}

							pivots = new GameObject[mesh.Length];
							if (model != null)
							{
								Destroy (model);
							}
							model = new GameObject ();
							model.transform.position = new Vector3 (0, -.8f, 0);
							for (int i = 0; i < mesh.Length; i++)
							{
								mesh [i].name = _orinames [i];
								mesh [i].transform.parent = model.transform;
								MFlash mf = mesh [i].AddComponent<MFlash> ();
								mf.vb = this;
								mf.index = i;
								lmfs.Add (mf);
								GameObject go = new GameObject ();
								go.transform.position = mesh [i].GetComponent<Renderer> ().bounds.center;
								go.transform.parent = mesh [i].transform;
								v2GetGizmo vgz = go.AddComponent<v2GetGizmo> ();
								vgz.vb = this;
								vgz.index = i;
								vgz.GC = GC;
								pivots [i] = go;
							}
							model.name = sname;
							status = "Load Successful";
							loading = false;
						}
					}
				}
			}
			else if (state == 8)
			{
				if (path != p_path && path != null)
				{
					p_path = path;
					if (path.Length > 0)
					{
						WWW www = new WWW (prefix + path [0]);
						yield return www;
						vdata [vchoice].particons [bchoice] = www.texture;
					}
				}
			}
			else if (state == 10)
			{
				//final export coroutine
				status = "Creating directories...";
				yield return new WaitForSeconds (.25f);
				//Create directory set up
				string root = Directory.CreateDirectory (Environment.CurrentDirectory + @"\" + sname.Trim ()).FullName + @"\"; 
				string[] root2 = new string[vdata.Count];
				for (int i = 0; i < vdata.Count; i++)
				{
					root2 [i] = Directory.CreateDirectory (root + i.ToString ()).FullName + @"\";
				}


				status = "Creating Icon.png";
				yield return new WaitForSeconds (.25f);
				File.WriteAllBytes (root + "Icon.png", (icon as Texture2D).EncodeToPNG ());

				status = "Creating Name List.txt";
				yield return new WaitForSeconds (.25f);

				string nlist = "";
				for (int i = 0; i < mesh.Length; i++)
				{
					nlist += "Object: " + mesh [i].name + Environment.NewLine;
					nlist += "X: " + pivots [i].transform.position.x.ToString () + Environment.NewLine;
					nlist += "Y: " + pivots [i].transform.position.y.ToString () + Environment.NewLine;
					nlist += "Z: " + pivots [i].transform.position.z.ToString ();

					if (i < mesh.Length - 1)
					{
						nlist += Environment.NewLine + Environment.NewLine;
					}
				}
				File.WriteAllText (root + "Name List.txt", nlist);

				status = "Creating getDirectoryList.php";
				yield return new WaitForSeconds (.25f);

				string php = @"<?php" + Environment.NewLine;
				php += @"$isFirst = true;" + Environment.NewLine;
				php += Environment.NewLine;
				php += @"foreach(glob('./*', GLOB_ONLYDIR) as $dir) {" + Environment.NewLine;
				php += @"    if (!$isFirst) {" + Environment.NewLine;
				php += @"    	echo "","";" + Environment.NewLine;
				php += @"    } else {" + Environment.NewLine;
				php += @"    	$isFirst = false;" + Environment.NewLine;
				php += @"    }" + Environment.NewLine;
				php += @"    echo basename($dir);" + Environment.NewLine;
				php += @"}";
				File.WriteAllText (root + "getDirectoryList.php", php);

				status = "Creating " + sname + ".obj";
				yield return new WaitForSeconds (.25f);

				File.WriteAllText (root + sname + ".obj", obj);

				status = "Creating " + mtlfilename;
				yield return new WaitForSeconds (.25f);

				File.WriteAllText (root + mtlfilename, mtl);

				status = "Creating textures";
				yield return new WaitForSeconds (.25f);

				for (int i = 0; i < otxpaths.Count; i++)
				{
					File.WriteAllBytes (root + otxpaths [i], (otextures [i] as Texture2D).EncodeToPNG ());
				}

				status = "Creating view files...";
				yield return new WaitForSeconds (.25f);

				for (int i = 0; i < vdata.Count; i++)
				{
					//create ViewName
					File.WriteAllText (root2 [i] + "ViewName.txt", vdata [i].viewname);

					string info = "";
					//create Textures
					for (int j = 0; j < vdata [i].partnames.Count; j++)
					{
						File.WriteAllBytes (root2 [i] + vdata [i].partnames [j] + ".png", (vdata [i].particons [j] as Texture2D).EncodeToPNG ());

						info += "Part: " + vdata [i].partnames [j] + Environment.NewLine;
						info += "System: " + vdata [i].systemnames [j] + Environment.NewLine;
						for (int k = 0; k < vdata [i].oindicesperpart [j].Count; k++)
						{
							info += "Object: " + vdata [i].oindicesperpart [j] [k].ToString () + Environment.NewLine;
						}
						info += "Description: " + vdata [i].descriptions [j];

						if (j < vdata [i].partnames.Count - 1)
						{
							info += Environment.NewLine + Environment.NewLine;
						}
					}

					File.WriteAllText (root2 [i] + "Info.txt", info);

					string medit = "";
					string[] smodes = new string[4];
					smodes [0] = "Opaque";
					smodes [1] = "Transparent";
					smodes [2] = "Transparent";
					smodes [3] = "Transparent";

					for (int j = 0; j < mesh.Length; j++)
					{
						for (int k = 0; k < vdata [i].vmats [j].Length; k++)
						{
							medit += "Object: " + j.ToString () + Environment.NewLine;
							medit += "Material Index: " + k.ToString () + Environment.NewLine;
							medit += "Shader Type: " + smodes [Mathf.RoundToInt (vdata [i].vmats [j] [k].GetFloat ("_Mode"))] + Environment.NewLine;
							medit += "Red: " + vdata [i].vmats [j] [k].color.r.ToString () + Environment.NewLine;
							medit += "Green: " + vdata [i].vmats [j] [k].color.g.ToString () + Environment.NewLine;
							medit += "Blue: " + vdata [i].vmats [j] [k].color.b.ToString () + Environment.NewLine;
							medit += "Alpha: " + vdata [i].vmats [j] [k].color.a.ToString ();

							if (j < mesh.Length - 1 || k < vdata [i].vmats [j].Length - 1)
							{
								medit += Environment.NewLine + Environment.NewLine;
							}
						}
					}
					File.WriteAllText (root2 [i] + "Mat Edit.txt", medit);
				}

				status = "Specimen folder exported successfully to " + Environment.CurrentDirectory + "\nRestarting application to clear memory...";

				state = 12;
				if (model != null)
				{
					Destroy (model);
				}
				sname = "";
				icon = null;
				ochoice = 0;
				mchoice = 0;
				vchoice = 0;
				bchoice = 0;
				_iconpath = "";
				opath = "";
				ostartindex = 0;
			}
			else if (state == 11)
			{
				//load specimen 
				string fpath = path [0];

				sname = Path.GetFileName (fpath);
				fpath += @"\";

				_iconpath = fpath + "icon.png";
				WWW www = new WWW (prefix + _iconpath);
				yield return www;
				icon = www.texture;

				status = "Loading obj text";
				yield return new WaitForSeconds (.25f);
				opath = fpath + sname + ".obj";

				www = new WWW (prefix + opath);
				while (!www.isDone)
				{
					upStatus (www);
					yield return null;
				}
				obj = www.text;
				string[] lines = getlines (www);
				_orinames = new List<string> ();
				for (int i = 0; i < lines.Length; i++)
				{
					if (lines [i].Contains ("mtllib"))
					{
						mtlfilename = lines [i].Substring (7);
					}

					if (lines [i].Contains ("usemtl"))
					{
						_orinames.Add (lines [i - 1].Substring (2));
					}
				}

				string basepath = opath.Substring (0, opath.Length - Path.GetFileName (opath).Length);
				string mtlpath = prefix + basepath + mtlfilename;
				www = new WWW (mtlpath);
				yield return www;
				mtl = www.text;
				lines = getlines (mtl);
				otextures = new List<Texture> ();
				otxpaths = new List<string> ();
				for (int i = 0; i < lines.Length; i++)
				{
					if (lines [i].Contains ("map_Kd"))
					{
						www = new WWW (prefix + basepath + lines [i].Substring (7));
						status = "Loading Textures";
						yield return www;
						if (!otxpaths.Contains (lines [i].Substring (7)))
						{
							otextures.Add (www.texture);
							otxpaths.Add (lines [i].Substring (7));
						}
					}
				}

				vdata = new List<ViewData> ();
				ObjReader.ObjData odata = new ObjReader.ObjData ();
				lmfs = new List<MFlash> ();
				odata = ObjReader.use.ConvertFileAsync (prefix + opath, true, opaque);
				while (!odata.isDone)
				{
					upStatus (odata);
					status = "Loading Model " + status;
					yield return null;
				}

				model = new GameObject ();
				model.transform.position = new Vector3 (0, -.8f, 0);
				status = "Setting Pivots";
				yield return new WaitForSeconds (.25f);
				mesh = odata.gameObjects;

				for (int i = 0; i < mesh.Length; i++)
				{
					Material[] cm = mesh [i].GetComponent<Renderer> ().materials;
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
					mesh [i].GetComponent<Renderer> ().materials = cm;
				}

				pivots = new GameObject[mesh.Length];
				www = new WWW (prefix + fpath + "Name List.txt");

				yield return www;
				lines = getlines (www);
				int s = 0;
				List<string> nl_Names = new List<string> ();
				List<Vector3> v_list = new List<Vector3> ();
				Vector3 vi = new Vector3 ();
				for (int i = 0; i < lines.Length; i++)
				{
					if (IsNullOrWhiteSpace (lines [i]))
						continue;
					if (s == 0)
					{
						nl_Names.Add (lines [i].Substring (8));
					}
					else if (s == 1)
					{
						float.TryParse (lines [i].Substring (3), out vi.x);
					}
					else if (s == 2)
					{
						float.TryParse (lines [i].Substring (3), out vi.y);
					}
					else if (s == 3)
					{
						float.TryParse (lines [i].Substring (3), out vi.z);
						v_list.Add (vi);
					}
					s++;
					if (s > 3)
					{
						s = 0;
					}
				}
				status = "Setting up object scripts";
				yield return new WaitForSeconds (.25f);
				for (int i = 0; i < mesh.Length; i++)
				{
					mesh [i].name = nl_Names [i];
					mesh [i].transform.parent = model.transform;
					MFlash mf = mesh [i].AddComponent<MFlash> ();
					mf.vb = this;
					mf.index = i;
					lmfs.Add (mf);
					GameObject go = new GameObject ();
					go.transform.position = v_list [i];
					go.transform.parent = mesh [i].transform;
					v2GetGizmo vgz = go.AddComponent<v2GetGizmo> ();
					vgz.vb = this;
					vgz.index = i;
					vgz.GC = GC;
					pivots [i] = go;
				}
				model.name = sname;

				status = "Retrieving View Path Data";
				yield return new WaitForSeconds (.25f);

				string[] vpaths = Directory.GetDirectories (fpath);
				for (int p = 0; p < vpaths.Length; p++)
				{
					vpaths [p] = Path.GetFileName (vpaths [p]) + @"\";
				}
				vdata = new List<ViewData> ();
				for (int i = 0; i < vpaths.Length; i++)
				{
					vdata.Add (new ViewData ());
					for (int x = 0; x < mesh.Length; x++)
					{
						Material[] tm = new Material[lmfs [x].orimat.Length];
						for (int y = 0; y < tm.Length; y++)
						{
							tm [y] = new Material (lmfs [x].orimat [y]);
						}
						vdata [vdata.Count - 1].vmats.Add (tm);
					}

					status = "Getting View Name";
					yield return new WaitForSeconds (.25f);

					www = new WWW (prefix + fpath + vpaths [i] + @"\ViewName.txt");
					yield return www;
					vdata [i].viewname = www.text;


					status = "Getting Info.txt";
					yield return new WaitForSeconds (.25f);

					www = new WWW (prefix + fpath + vpaths [i] + @"\Info.txt");
					yield return www;
					lines = getlines (www);
					s = 0;
					int iterator = 0;
					for (int x = 0; x < lines.Length; x++)
					{
						if (IsNullOrWhiteSpace (lines [x]))
							continue;
						if (s == 0)
						{
							vdata [i].partnames.Add (lines [x].Substring (6));
							vdata [i].oindicesperpart.Add (new List<int> ());
						}
						else if (s == 1)
						{
							vdata [i].systemnames.Add (lines [x].Substring (8));
						}
						else if (s == 2)
						{
							int num = 0;
							bool isnum = false;
							isnum = Int32.TryParse (lines [x].Substring (8), out num);
							if (isnum)
								vdata [i].oindicesperpart [iterator].Add (num);
							else
								s++;
						}
						if (s == 3)
						{
							vdata [i].descriptions.Add (lines [x].Substring (13));
							iterator++;
						}
						if (s != 2)
						{
							s++;
						}
						if (s >= 3)
							s = 0;
					}
					iterator = 0;
					status = "Getting Part Icons";
					yield return new WaitForSeconds (.25f);

					for (int x = 0; x < vdata [i].partnames.Count; x++)
					{
						www = new WWW (prefix + fpath + vpaths [i] + vdata [i].partnames [x] + ".png");
						yield return www;
						vdata [i].particons.Add (www.texture);
					}

					status = "Getting Mat Edit";
					yield return new WaitForSeconds (.25f);

					www = new WWW (prefix + fpath + vpaths [i] + @"\Mat Edit.txt");
					yield return www;
					lines = getlines (www);
					s = 0;
					int oid = 0;
					int mid = 0;
					string mtype = "Opaque";
					Color c = Color.white;
					for (int x = 0; x < lines.Length; x++)
					{
						if (IsNullOrWhiteSpace (lines [x]))
							continue;
						if (s == 0)
						{
							Int32.TryParse (lines [x].Substring (8), out oid);
						}
						else if (s == 1)
						{
							Int32.TryParse (lines [x].Substring (16), out mid);
						}
						else if (s == 2)
						{
							mtype = lines [x].Substring (13);
						}
						else if (s == 3)
						{
							float f = 0;
							float.TryParse (lines [x].Substring (5), out f);
							c.r = f;
						}
						else if (s == 4)
						{
							float f = 0;
							float.TryParse (lines [x].Substring (7), out f);
							c.g = f;
						}
						else if (s == 5)
						{
							float f = 0;
							float.TryParse (lines [x].Substring (6), out f);
							c.b = f;
						}
						else if (s == 6)
						{
							float f = 0;
							float.TryParse (lines [x].Substring (7), out f);
							c.a = f;
							Material tmat = new Material (opaque);
							if (mtype == "Transparent")
							{
								tmat = new Material (transparent);
							}
							else
							{
								tmat = new Material (opaque);
							}
							tmat.color = c;
							tmat.mainTexture = vdata [i].vmats [oid] [mid].mainTexture;
							vdata [i].vmats [oid] [mid] = tmat;
							//print (vdata [i].vmats [oid] [mid].color);
						}
						s++;
						if (s > 6)
							s = 0;
					}
				}
				state = 5;
			} // end of state 11

			else if (state == 12)
			{
				yield return new WaitForSeconds (7);
				state = 0;
			}

			yield return null;
		} //end of coroutine loop
		StartCoroutine(Start());
	}

	void OnGUI()
	{
		int h = Screen.height;
		int w = Screen.width;
		GUI.skin.label.fontSize = Mathf.RoundToInt (Screen.height * .03f);
		GUI.skin.button.fontSize = Mathf.RoundToInt (Screen.height * .03f);
		GUI.skin.textField.fontSize = Mathf.RoundToInt (Screen.height * .03f);
		Vector3 mp = Input.mousePosition;
		mp.y = h - mp.y;
		centerStyle.fontSize = Mathf.RoundToInt (Screen.height * .03f);
		if (state == 0)
		{
			GUILayout.Label (status);
			if (GUILayout.Button ("New Specimen", GUI.skin.button, GUILayout.Width(w*.2f)))
			{
				state = 1;
			}
			if (GUILayout.Button ("Load Specimen", GUI.skin.button, GUILayout.Width(w*.2f)))
			{
				path = StandaloneFileBrowser.OpenFolderPanel ("Load Specimen Folder", "", false);
				//state = 11;
			}
		}
		else if (state == 1)
		{
			GUILayout.Label ("Enter Specimen Name", GUI.skin.label);
			sname = GUILayout.TextField (sname, GUI.skin.textField);
			if (GUILayout.Button ("Confirm", GUI.skin.button, GUILayout.Width(w*.3f)))
			{
				if (sname.Length > 0)
				{
					state = 2;
				}	
			}
			if (GUILayout.Button ("Cancel", GUI.skin.button, GUILayout.Width(w*.3f)))
			{
				vdata = new List<ViewData> ();
				state = 0;
				if (model != null)
				{
					Destroy (model);
				}
				sname = "";
				icon = null;
				ochoice = 0;
				mchoice = 0;
				vchoice = 0;
				bchoice = 0;
				_iconpath = "";
				opath = "";
				ostartindex = 0;
			}
		}
		else if (state == 2)
		{
			GUILayout.Label (status, GUI.skin.label);
			GUILayout.Label ("Load Specimen Button Icon", GUI.skin.label);
			GUILayout.TextField (_iconpath, GUI.skin.textField);
			Rect r_icon = new Rect (w * .005f, h * .4f, h * .2f, h * .2f);
			if (GUILayout.Button ("Load Icon", GUI.skin.button, GUILayout.Width(w*.3f)))
			{
				path = StandaloneFileBrowser.OpenFilePanel ("Load Icon", "", "png", false);
			}
			if (GUILayout.Button ("Confirm", GUILayout.Width(w*.3f)) && icon != null)
			{
				state = 3;
			}
			if (GUILayout.Button ("Go Back", GUILayout.Width(w*.3f)))
			{
				state = 1;
			}
			if (icon != null)
			{
				GUI.DrawTexture (r_icon, icon);
			}
		}
		else if (state == 3)
		{
			GUILayout.Label (status, GUI.skin.label);
			GUILayout.Label ("Load Specimen Model", GUI.skin.label);
			GUILayout.TextField (opath, GUI.skin.textField);
			if (!loading)
			{
				if (GUILayout.Button ("Load OBJ", GUI.skin.button, GUILayout.Width(w*.3f)))
				{
					loading = true;
					path = StandaloneFileBrowser.OpenFilePanel ("Load OBJ", "", "obj", false);
				}

				if (GUILayout.Button ("Confirm", GUI.skin.button, GUILayout.Width(w*.3f)))
				{
					if (!loading && model != null && mesh != null)
					{
						state = 4;
					}
				}
				
				if (GUILayout.Button ("Go Back", GUI.skin.button, GUILayout.Width(w*.3f)))
				{
					state = 2;
				}
			}
		}
		else if (state == 4)
		{
			GUILayout.Label ("Set object names and GUI pointer locations.", GUI.skin.label);


			if (ochoice <= -1)
			{
				GUILayout.Label ("Showing all", GUI.skin.label);
			}
			else
			{
				Rect r_sel = new Rect (0, 0, h * .04f, h * .04f);
				Vector3 tempv = Camera.main.WorldToScreenPoint (pivots [ochoice].transform.position);
				r_sel.x = tempv.x - r_sel.width * .5f;
				r_sel.y = h - tempv.y - r_sel.height * .5f;
				GUI.color = new Color (1, 1, 1, .5f);
				GUI.DrawTexture (r_sel, t_select);
				GUI.color = Color.white;
				GUILayout.Label (mesh [ochoice].name + " #" + (ochoice + 1).ToString () + " of " + mesh.Length.ToString (), GUI.skin.label);
			}
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (etleft, GUILayout.Width(w*.15f)))
			{
				if (ochoice > -1)
				{
					ochoice--;
				}
			}
			if (GUILayout.Button (etright, GUILayout.Width(w*.15f)))
			{
				if (ochoice < mesh.Length - 1)
					ochoice++;
			}
			GUILayout.EndHorizontal ();
			if (ochoice >= 0)
			{
				mesh [ochoice].name = GUILayout.TextField (mesh [ochoice].name, GUI.skin.textField);
				if (GUILayout.Button ("Revert", GUI.skin.button, GUILayout.Width(w*.3f)))
				{
					mesh [ochoice].name = _orinames [ochoice];
				}
			}
			if (GUILayout.Button ("Confirm", GUI.skin.button, GUILayout.Width(w*.3f)))
			{
				state = 5;
			}
			if (GUILayout.Button ("Go Back", GUI.skin.button, GUILayout.Width(w*.3f)))
			{
				state = 3;
			}

			Rect botrect = new Rect ();
			botrect.x = w * .5f;
			botrect.y = h * .95f;
			if (ochoice < 0)
			{
			}
			else
			{
				GUI.Label (botrect, mesh [ochoice].name, centerStyle);
			}
		}
		else if (state == 5)
		{
			if (vchoice > vdata.Count)
				vchoice = 0;
			//view selection menu + finalize option
			if (vdata.Count > 0)
			{
				GUILayout.Label ("Showing View " + (vchoice + 1).ToString () + " of " + vdata.Count);
				vdata [vchoice].viewname = GUILayout.TextField (vdata [vchoice].viewname, GUI.skin.textField);
			}
			else
			{
				GUILayout.Label ("No Views Created", GUI.skin.label);
			}
			if (vdata.Count > 0)
			{
				if (GUILayout.Button ("Edit", GUI.skin.button, GUILayout.Width(w*.2f)))
				{
					for (int i = 0; i < lmfs.Count; i++)
					{
						lmfs [i].Change (vdata [vchoice].vmats [i]);
					}
					state = 6;
				}
			}
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (etleft, GUI.skin.button, GUILayout.Width(w*.1f)))
			{
				if (vchoice > 0)
					vchoice--;
			}
			if (GUILayout.Button (etright, GUI.skin.button, GUILayout.Width(w*.1f)))
			{
				if (vchoice < vdata.Count - 1)
				{
					vchoice++;
				}
			}
			GUILayout.EndHorizontal ();
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("-", GUI.skin.button, GUILayout.Width(w*.1f)))
			{
				vdata.Remove (vdata [vchoice]);
				vchoice = 0;
				if (vdata.Count > 0)
					vchoice--;
				if (vchoice < 0)
					vchoice = 0;
			}
			if (GUILayout.Button ("+", GUI.skin.button, GUILayout.Width(w*.1f)))
			{
				vdata.Add (new ViewData ());
				vchoice = vdata.Count - 1;
				vdata [vchoice].viewname = "View #" + vchoice.ToString ();
				for (int i = 0; i < mesh.Length; i++)
				{
					Material[] tm = new Material[lmfs [i].orimat.Length];
					for (int x = 0; x < tm.Length; x++)
					{
						tm [x] = new Material (lmfs [i].orimat [x]);
					}
					vdata [vchoice].vmats.Add (tm);
				}
			}
			GUILayout.EndHorizontal ();
			if (GUILayout.Button ("Finish and Export", GUILayout.Width(w*.2f)) && vdata.Count > 0)
			{
				for (int i = 0; i < vdata.Count; i++)
				{
					if (vdata [i].partnames.Count > 0)
					{
						state = 10;
						return;
					}
				}
			}
			if (GUILayout.Button ("Go Back", GUILayout.Width(w*.2f)))
			{
				state = 4;
			}
		}
		else if (state == 6)
		{
			GUILayout.Label (vdata [vchoice].viewname, GUI.skin.label);
			if (GUILayout.Button ("Create Body Parts", GUI.skin.button, GUILayout.Width(w*.2f)))
			{
				state = 7;
			}
			if (GUILayout.Button ("Edit Materials", GUI.skin.button, GUILayout.Width(w*.2f)))
			{
				state = 9;
			}
			if (GUILayout.Button ("Go Back", GUI.skin.button, GUILayout.Width(w*.2f)))
			{
				state = 5;
			}
		}
		else if (state == 7)
		{
			GUILayout.Label (vdata [vchoice].viewname, GUI.skin.label);
			GUILayout.Label ("Body Part Creation", GUI.skin.label);
			if (bchoice >= vdata [vchoice].partnames.Count)
			{
				bchoice = 0;
			}
			if (GUILayout.Button ("Add New Body Part", GUI.skin.button, GUILayout.Width(w*.3f)))
			{
				vdata [vchoice].AddPart ();
				bchoice = vdata [vchoice].partnames.Count - 1;
				state = 8;
			}
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (etleft, GUILayout.Width(w*.15f)))
			{
				if (bchoice > 0)
					bchoice--;
			}
			if (GUILayout.Button (etright, GUILayout.Width(w*.15f)))
			{
				if (bchoice < vdata [vchoice].partnames.Count - 1)
					bchoice++;
			}
			GUILayout.EndHorizontal ();

			if (GUILayout.Button ("Edit Body Part", GUI.skin.button, GUILayout.Width(w*.3f)) && vdata [vchoice].partnames.Count > 0)
			{
				state = 8;
			}
			if (GUILayout.Button ("Delete Body Part", GUI.skin.button, GUILayout.Width(w*.3f)) && vdata [vchoice].partnames.Count > 0)
			{
				vdata [vchoice].RemovePartAt (bchoice);
				bchoice = 0;
			}
			if (GUILayout.Button ("Go Back", GUI.skin.button, GUILayout.Width(w*.3f)))
			{
				state = 6;
			}
			//if there is 1 or more body parts, display their properties
			if (vdata [vchoice].partnames.Count > 0)
			{
				GUI.skin.textField.wordWrap = true;
				Rect rlabel = new Rect (0, h * .1f, w * .5f, 0);
				Rect rdata = new Rect (w * .5f, h * .1f, w * .5f, 0);
				rlabel.height = GUI.skin.label.CalcHeight (new GUIContent ("F"), w);
				rdata.height = GUI.skin.label.CalcHeight (new GUIContent (vdata [vchoice].partnames [bchoice]), rdata.width);
				GUIStyle rlstyle = new GUIStyle (GUI.skin.label);
				rlstyle.alignment = TextAnchor.UpperRight;
				GUI.Label (rlabel, "Part: ", rlstyle);
				vdata [vchoice].partnames [bchoice] = GUI.TextField (rdata, vdata [vchoice].partnames [bchoice], GUI.skin.textField);

				rdata.y += rdata.height;
				rlabel.y = rdata.y;
				rdata.height = GUI.skin.textField.CalcHeight (new GUIContent (vdata [vchoice].systemnames [bchoice]), rdata.width);
				GUI.Label (rlabel, "System: ", rlstyle);
				vdata [vchoice].systemnames [bchoice] = GUI.TextField (rdata, vdata [vchoice].systemnames [bchoice], GUI.skin.textField);

				rdata.y += rdata.height;
				rdata.height = GUI.skin.textField.CalcHeight (new GUIContent (vdata [vchoice].descriptions [bchoice]), rdata.width);
				rlabel.y = rdata.y;
				GUI.Label (rlabel, "Description: ", rlstyle);
				vdata [vchoice].descriptions [bchoice] = GUI.TextField (rdata, vdata [vchoice].descriptions [bchoice], GUI.skin.textField);
				GUI.skin.textField.wordWrap = false;
				rdata.y += rdata.height;
				rdata.height = GUI.skin.label.CalcHeight (new GUIContent ("F"), w);
				rdata.width = w * 5;
				for (int i = 0; i < vdata [vchoice].oindicesperpart [bchoice].Count; i++)
				{
					GUI.Label (rdata, mesh [vdata [vchoice].oindicesperpart [bchoice] [i]].name, GUI.skin.label);
					rdata.y += rdata.height;
				}
				if (vdata [vchoice].particons [bchoice] != null)
				{
					GUILayout.Label (vdata [vchoice].particons [bchoice], GUILayout.Width (h * .1f), GUILayout.Height (h * .1f));
				}
			}

		}
		else if (state == 8)
		{
			
			GUILayout.Label (vdata [vchoice].viewname, GUI.skin.label);

			GUILayout.Label ("Body Part Creation\nSeparate multiple system names With, Commas", GUI.skin.label);

			if (GUILayout.Button ("Confirm", GUILayout.Width(w*.3f)) && vdata [vchoice].partnames [bchoice].Length > 0 && vdata [vchoice].systemnames [bchoice].Length > 0 && vdata [vchoice].descriptions [bchoice].Length > 0 && vdata [vchoice].particons [bchoice] != null && vdata [vchoice].oindicesperpart [bchoice].Count > 0)
			{
				state = 7;
			}

			if (GUILayout.Button ("Delete Part And Go Back", GUILayout.Width(w*.3f)))
			{
				state = 7;
				vdata [vchoice].RemovePartAt (bchoice);
				bchoice = 0;
				return;
			}
			if (vdata [vchoice].particons [bchoice] != null)
			{
				GUILayout.Label (vdata [vchoice].particons [bchoice], GUILayout.Width (h * .1f), GUILayout.Height (h * .1f));
			}
			else
			{
				GUI.color = Color.red;
				GUILayout.Label ("Choose Icon");
				GUI.color = Color.white;
			}

			if (GUILayout.Button ("Load Icon", GUI.skin.button, GUILayout.Width(w*.3f)))
			{
				path = StandaloneFileBrowser.OpenFilePanel ("Load Icon", "", "png", false);
			}
			GUI.skin.textField.wordWrap = true;
			Rect rlabel = new Rect (0, h * .1f, w * .5f, 0);
			Rect rdata = new Rect (w * .5f, h * .1f, w * .5f, 0);
			rlabel.height = GUI.skin.label.CalcHeight (new GUIContent ("F"), w);
			rdata.height = GUI.skin.label.CalcHeight (new GUIContent (vdata [vchoice].partnames [bchoice]), rdata.width);
			GUIStyle rlstyle = new GUIStyle (GUI.skin.label);
			rlstyle.alignment = TextAnchor.UpperRight;
			GUI.Label (rlabel, "Part: ", rlstyle);
			vdata [vchoice].partnames [bchoice] = GUI.TextField (rdata, vdata [vchoice].partnames [bchoice], GUI.skin.textField);

			rdata.y += rdata.height;
			rlabel.y = rdata.y;
			rdata.height = GUI.skin.textField.CalcHeight (new GUIContent (vdata [vchoice].systemnames [bchoice]), rdata.width);
			GUI.Label (rlabel, "System: ", rlstyle);
			vdata [vchoice].systemnames [bchoice] = GUI.TextField (rdata, vdata [vchoice].systemnames [bchoice], GUI.skin.textField);

			rdata.y += rdata.height;
			rdata.height = GUI.skin.textField.CalcHeight (new GUIContent (vdata [vchoice].descriptions [bchoice]), rdata.width);
			rlabel.y = rdata.y;
			GUI.Label (rlabel, "Description: ", rlstyle);
			vdata [vchoice].descriptions [bchoice] = GUI.TextField (rdata, vdata [vchoice].descriptions [bchoice], GUI.skin.textField);
			GUI.skin.textField.wordWrap = false;
			rdata.y += rdata.height;
			rdata.height = GUI.skin.label.CalcHeight (new GUIContent ("F"), w);
			rdata.width = w * 5;
			for (int i = 0; i < vdata [vchoice].oindicesperpart [bchoice].Count; i++)
			{
				if (vdata [vchoice].oindicesperpart [bchoice] [i] == ochoice)
					GUI.color = Color.yellow;
				GUI.Label (rdata, mesh [vdata [vchoice].oindicesperpart [bchoice] [i]].name, GUI.skin.label);
				GUI.color = Color.white;
				rdata.y += rdata.height;
			}
			if (vdata [vchoice].oindicesperpart [bchoice].Count == 0)
			{
				GUI.color = Color.red;
				GUI.Label (rdata, "Add an object...");
				GUI.color = Color.white;
			}

			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (etleft, GUI.skin.button, GUILayout.Width(w*.15f)))
			{
				if (ochoice >= 0)
					ochoice--;
			}
			if (GUILayout.Button (etright, GUI.skin.button, GUILayout.Width(w*.15f)))
			{
				if (ochoice < mesh.Length - 1)
					ochoice++;
			}
			GUILayout.EndHorizontal ();
			if (ochoice < 0)
			{
				GUI.color = new Color (1, 1, 1, .5f);
				GUILayout.Button ("Add Object", GUI.skin.button, GUILayout.Width(w*.3f));
				GUI.color = Color.white;
			}
			else if (vdata [vchoice].oindicesperpart [bchoice].Contains (ochoice))
			{
				if (GUILayout.Button ("Remove Object", GUI.skin.button, GUILayout.Width(w*.3f)))
				{
					vdata [vchoice].oindicesperpart [bchoice].Remove (ochoice);
				}
			}
			else
			{
				if (GUILayout.Button ("Add Object", GUI.skin.button, GUILayout.Width(w*.3f)))
				{
					vdata [vchoice].oindicesperpart [bchoice].Add (ochoice);
				}
			}
			if (ochoice >= 0)
			{
				GUILayout.Label ("Object " + (ochoice + 1).ToString () + " of " + mesh.Length.ToString () + " " + mesh[ochoice].name);
			}
			Rect botrect = new Rect ();
			botrect.x = w * .5f;
			botrect.y = h * .95f;
			if (ochoice < 0)
			{
			}
			else
			{
				if (vdata [vchoice].oindicesperpart [bchoice].Contains (ochoice))
					GUI.color = Color.yellow;
				GUI.Label (botrect, mesh [ochoice].name, centerStyle);
				GUI.color = Color.white;
			}
		}
		else if (state == 9)
		{
			GUILayout.Label (vdata [vchoice].viewname, GUI.skin.label);
			GUILayout.Label ("Material Editing", GUI.skin.label);
			if (GUILayout.Button ("Show Full", GUI.skin.button, GUILayout.Width (w * .3f)))
			{
				ochoice = -1;
			}
			if (GUILayout.Button ("Go Back", GUI.skin.button, GUILayout.Width(w*.3f)))
			{
				state = 6;
			}
			if (ochoice >= 0)
			{
				GUILayout.Label ("Object " + (ochoice + 1).ToString () + " of " + mesh.Length.ToString () + " " + mesh[ochoice].name);
			}
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (etleft, GUI.skin.button, GUILayout.Width(w*.15f)))
			{
				if (ochoice >= 0)
				{
					ochoice--;
					mchoice = 0;
					if(ochoice>=0)
						ccolor = vdata [vchoice].vmats [ochoice] [mchoice].color;
				}
			}
			if (GUILayout.Button (etright, GUI.skin.button, GUILayout.Width(w*.15f)))
			{
				if (ochoice < mesh.Length - 1)
				{
					ochoice++;
					mchoice = 0;
					ccolor = vdata [vchoice].vmats [ochoice] [mchoice].color;
				}
			}
			GUILayout.EndHorizontal ();


			if (ochoice >= 0)
			{
				GUILayout.Label ("Mat index: " + mchoice.ToString ()); 
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("-", GUILayout.Width(w*.15f)))
				{
					if (mchoice > 0)
					{
						mchoice--;
						ccolor = vdata [vchoice].vmats [ochoice] [mchoice].color;
					}
				}
				if (GUILayout.Button ("+", GUILayout.Width(w*.15f)))
				{
					if (mchoice < vdata [vchoice].vmats [ochoice].Length - 1)
					{
						mchoice++;
						ccolor = vdata [vchoice].vmats [ochoice] [mchoice].color;
					}
				}
				GUILayout.EndHorizontal ();
				string _type = "Transparent";
				if (vdata [vchoice].vmats [ochoice] [mchoice].GetFloat ("_Mode") == 0)
				{
					_type = "Opaque";
				}
				if (GUILayout.Button ("Shader Type: " + _type, GUILayout.Width(w*.3f)))
				{
					Material target = new Material (vdata [vchoice].vmats [ochoice] [mchoice]);
					Material tmat = new Material (opaque);
					if (_type == "Opaque")
					{
						//if opaque, make it transparent
						tmat = new Material (transparent);
					}
					tmat.mainTexture = target.mainTexture;
					tmat.color = target.color;
					vdata [vchoice].vmats [ochoice] [mchoice] = tmat;
					lmfs [ochoice].Change (vdata [vchoice].vmats [ochoice]);
				}
				Color tcolor = ccolor;

				GUILayout.Label ("Red " + Mathf.RoundToInt(vdata [vchoice].vmats [ochoice] [mchoice].color.r*255).ToString());
				tcolor.r = GUILayout.HorizontalSlider (vdata [vchoice].vmats [ochoice] [mchoice].color.r, 0f, 1f, GUILayout.Width(w*.3f));

				GUILayout.Label ("Green " + Mathf.RoundToInt(vdata [vchoice].vmats [ochoice] [mchoice].color.g*255).ToString());
				tcolor.g = GUILayout.HorizontalSlider (vdata [vchoice].vmats [ochoice] [mchoice].color.g, 0f, 1f, GUILayout.Width(w*.3f));

				GUILayout.Label ("Blue " + Mathf.RoundToInt(vdata [vchoice].vmats [ochoice] [mchoice].color.b*255).ToString());
				tcolor.b = GUILayout.HorizontalSlider (vdata [vchoice].vmats [ochoice] [mchoice].color.b, 0f, 1f, GUILayout.Width(w*.3f));

				GUILayout.Label ("Alpha " + Mathf.RoundToInt(vdata [vchoice].vmats [ochoice] [mchoice].color.a*255).ToString());
				tcolor.a = GUILayout.HorizontalSlider (vdata [vchoice].vmats [ochoice] [mchoice].color.a, 0f, 1f, GUILayout.Width(w*.3f));

				if (ccolor != tcolor)
				{
					Color ncolor = tcolor;
					if (ncolor.r < 1f && ncolor.r >= .98f)
						ncolor.r = 1f;
					
					if (ncolor.g < 1f && ncolor.g >= .98f)
						ncolor.g = 1f;
					
					if (ncolor.b < 1f && ncolor.b >= .98f)
						ncolor.b = 1f;
					
					if (ncolor.a < 1f && ncolor.a >= .98f)
						ncolor.a = 1f;

					if (ncolor.r < .01f)
						ncolor.r = 0f;
					if (ncolor.g < .01f)
						ncolor.g = 0f;
					if (ncolor.b < .01f)
						ncolor.b = 0f;
					if (ncolor.a < .01f)
						ncolor.a = 0f;
					
					vdata [vchoice].vmats [ochoice] [mchoice].color = ncolor;
					lmfs [ochoice].Change (vdata [vchoice].vmats [ochoice]);
				}

				GUI.color = new Color (0, 0, 0, 0);
				GUILayout.Button ("Shader Type: Transparent", GUILayout.Width(w*.3f));
				GUI.color = new Color (1, 1, 1, 1);
			}
		}
		else if (state == 10 || state == 11 || state == 12)
		{
			GUILayout.Label (status);
		}
	}


	void upStatus(WWW w)
	{
		status = (w.progress*100).ToString ("0") + "%";
	}
	void upStatus(ObjReader.ObjData o)
	{
		status = (o.progress * 100).ToString ("0") + "%";
	}

	bool IsNullOrWhiteSpace(string value)
	{
		return String.IsNullOrEmpty (value) || value.Trim ().Length == 0;
	}

	string[] getlines(WWW www)
	{
		char[] eliminate = new char[]{ '\n', '\r' };
		string splitText = www.text;
		return splitText.Split (eliminate, System.StringSplitOptions.RemoveEmptyEntries);
	}

	string[] getlines(string s)
	{
		char[] eliminate = new char[]{ '\n', '\r' };
		string splitText = s;
		return splitText.Split (eliminate, System.StringSplitOptions.RemoveEmptyEntries);
	}
}
