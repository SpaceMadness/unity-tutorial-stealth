using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using LunarPlugin;

[CCommand("enable")]
class Cmd_enable : EnableDisableCommand
{
	public Cmd_enable()
		: base(true)
	{
	}
}

[CCommand("disable")]
class Cmd_disable : EnableDisableCommand
{
	public Cmd_disable()
		: base(false)
	{
	}
}

class EnableDisableCommand : CPlayModeCommand // only available in play mode
{
	delegate void EnableDisableDelegate (bool flag); // contains logic for enabling/disabling item

	private readonly IDictionary<string, EnableDisableDelegate> lookup; // items lookup
	private readonly bool flag; // enable or disable?

	public EnableDisableCommand (bool flag)
	{
		this.lookup = new Dictionary<string, EnableDisableDelegate> ();
		this.flag = flag;

		// cameras
		lookup.Add("cameras", delegate(bool enabled)
		{
			// 1. find all cameras
			GameObject[] cameras = GameObject.FindGameObjectsWithTag("Camera");

			// 2.enable/disable
			foreach (GameObject camera in cameras)
			{
				camera.GetComponent<MeshCollider>().enabled = enabled;
				camera.GetComponent<Light>().color = enabled ? Color.red : Color.green;
			}
		});

		// laser fences
		lookup.Add("laserFences", delegate(bool enabled)
		{
			// 1. find all cameras
			GameObject[] fences = GameObject.FindGameObjectsWithTag("LaserFence");
			
			// 2.enable/disable
			foreach (GameObject fence in fences)
			{
				fence.GetComponent<BoxCollider>().enabled = enabled;
				fence.GetComponent<MeshRenderer>().material.color = enabled ? Color.red : Color.green;
			}
		});
	}

	void Execute (string name)
	{
		EnableDisableDelegate del;
		if (lookup.TryGetValue (name, out del)) {
			del (flag);
			PrintIndent ("'{0}' enabled: {1}", name, flag.ToString ());
		} else {
			PrintError ("Unknown item: '{0}'", name);
		}
	}

	protected override IList<string> AutoCompleteArgs (string commandLine, string token)
	{
		List<string> suggestions = new List<string>();

		// find items
		foreach (string name in lookup.Keys)
		{
			if (name.StartsWith(token, StringComparison.OrdinalIgnoreCase))
			{
				suggestions.Add(name);
			}
		}

		// sort items
		suggestions.Sort();

		return suggestions;
	}
}