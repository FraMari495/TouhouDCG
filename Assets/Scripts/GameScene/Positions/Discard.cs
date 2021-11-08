using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;



namespace Position
{
    public class Discard : PositionBase<Discard>
    {
        protected override PosEnum Pos => PosEnum.Discard;
    }
}
