﻿using FainCraft.Gameplay.WorldScripts.Data;

namespace FainCraft.Gameplay.WorldScripts.Systems.Rendering.Lighting;
public interface ILightingCalculator
{
    public void Calculate(LightingRegionData data);
}