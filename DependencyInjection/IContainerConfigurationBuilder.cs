﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOF.Framework.DependencyInjection
{
    public interface IContainerConfigurationBuilder
    {
        void LoadFromConfiguration();
    }
}
