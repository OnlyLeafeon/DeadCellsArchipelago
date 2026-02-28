using dc.level.@struct;
using static DeadCellsArchipelago.RuneManager;

namespace DeadCellsArchipelago {
    public static class RoomManager
    {
        public static void InitializeRoomHooks()
        {
            Hook_PrisonCourtyard.buildMainRooms += (orig, self) => { useOriginalHasPermanentItem=false; var res=orig(self); useOriginalHasPermanentItem=true; return res;};
            Hook_PrisonRoof.buildMainRooms += (orig, self) => { useOriginalHasPermanentItem=false; var res=orig(self); useOriginalHasPermanentItem=true; return res;};
            Hook_Crypt.finalize += (orig, self) => { useOriginalHasPermanentItem=false;orig(self); useOriginalHasPermanentItem=true; };
            Hook_SewerShort.buildSecondaryRooms += (orig, self) => { useOriginalHasPermanentItem=false; orig(self); useOriginalHasPermanentItem=true; };
            Hook_AncientTemple.buildSecondaryRooms += (orig, self) => { useOriginalHasPermanentItem=false; orig(self); useOriginalHasPermanentItem=true; };
            Hook_Ossuary.buildMainRooms += (orig, self) => { useOriginalHasPermanentItem=false; var res=orig(self); useOriginalHasPermanentItem=true; return res;};
            Hook_Ossuary.buildSecondaryRooms += (orig, self) => { useOriginalHasPermanentItem=false; orig(self); useOriginalHasPermanentItem=true; };
        }
    }
}