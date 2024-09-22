using Mantodea.Content.Components;

namespace Mantodea.Content.Inventories
{
    public class CommonInventory : Container
    {
        public CommonInventory(int width, int height, int slotSize = 16) 
        {
            InventorySlots = new int[width, height];

            SlotSize = slotSize;
        }

        /// <summary>
        /// 0-Unused -1-Locked -2-Unusable Other-ItemID
        /// </summary>
        public int[,] InventorySlots;

        public int SlotSize;

        public override int Width => SlotSize * InventorySlots.GetLength(0);

        public override int Height => SlotSize * InventorySlots.GetLength(1);
    }
}
