using UnityEngine;
using System.Collections;
 
//purpose
//when a hit occurs, the hit target is saved so no additional hits occur until this is reset
public class dmSource : MonoBehaviour {
	public ArrayList hitTransforms;
	void Start()
	{
		hitTransforms = new ArrayList();
		hitTransforms.Add(transform);
	}
	public bool wasHit(Transform victim) //public function for looking up if a victim is listed
	{
		if(hitTransforms!=null)
		{
			hitTransforms.TrimToSize();
			if(hitTransforms.Capacity>0)
			{
				foreach(Object i in hitTransforms)
				{
					if(i == victim)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void resetDamage() //resets list
	{
		hitTransforms = new ArrayList();
		hitTransforms.Add(transform);
	}
}
