﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vibrant.Tsdb
{
   public enum Publish
   {
      Nowhere = 0x00,
      Locally = 0x01,
      Remotely = 0x02,
      LocallyAndRemotely = Locally | Remotely,
   }
}
