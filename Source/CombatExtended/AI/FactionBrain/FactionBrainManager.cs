﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace CombatExtended
{
	public class FactionBrainManager : MapComponent
	{
		private Dictionary<Faction, FactionBrain> brains = new Dictionary<Faction, FactionBrain>();

		public FactionBrainManager(Map map) : base(map)
		{
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.LookDictionary(ref brains, "brains", LookMode.Deep);
			Action action = delegate
			{
				foreach (var fac in brains)
				{
					fac.Value.manager = this;
				}
			};
			LongEventHandler.ExecuteWhenFinished(action);
		}

		public override void MapComponentTick()
		{
			foreach (var entry in Find.FactionManager.AllFactions)
			{
				var brain = this.GetBrainFor(entry);

				brain.BrainTick();
			}
		}

		public FactionBrain GetBrainFor(Faction fac)
		{
			FactionBrain brain;
			if (!brains.TryGetValue(fac, out brain))
			{
				// Create new brain if we don't already have one
				brain = new FactionBrain(this, fac);
				brains.Add(fac, brain);
			}
			return brain;
		}
	}
}