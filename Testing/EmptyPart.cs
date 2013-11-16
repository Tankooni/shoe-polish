using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using Punk.Masks;
using Punk;

namespace Testing
{
    class EmptyPart : PartBase
    {
        private float Distance;
        private float angle;
        public float parentCol;
        public float parentRow;

        public EmptyPart(Entity e, int Col, int Row, int PCol, int PRow) : base(e, Col, Row)
        {
            Type = "EmptyPart";

            curCol = Col;
            curRow = Row;
            parentCol = PCol;
            parentRow = PRow;

            int A = (32 * Col) * (32 * Col);
            int B = (32 * Row) * (32 * Row);
            float C = (float)Math.Sqrt(A + B);
            Distance = C;
            angle = (float)Math.Atan2(Col, Row) * (180 / (float)Math.PI);
            myShip = e as Ship;
            myShip.shipParts.Add(this);

            SetHitbox(32, 32);

            X = 100;
            Y = 100;
        }

        public override void Update()
        {
            base.Update();


            CenterOrigin();

            FP.AnchorTo(ref X, ref Y, myShip.X, myShip.Y, Distance, Distance);
            FP.RotateAround(ref X, ref Y, myShip.X, myShip.Y, myShip.ShipCenter.Angle + angle, false);

        }
    }
}
