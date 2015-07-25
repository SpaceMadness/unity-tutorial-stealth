﻿using UnityEngine;

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
}