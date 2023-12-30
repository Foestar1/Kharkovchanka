﻿using System;

namespace NWH.VehiclePhysics2.Modules.ESC
{
    /// <summary>
    ///     MonoBehaviour wrapper for Electronic Stability Control (ESC) module.
    /// </summary>
    [Serializable]
    public partial class ESCModuleWrapper : ModuleWrapper
    {
        public ESCModule module = new ESCModule();


        public override VehicleComponent GetModule()
        {
            return module;
        }


        public override void SetModule(VehicleComponent module)
        {
            this.module = module as ESCModule;
        }
    }
}