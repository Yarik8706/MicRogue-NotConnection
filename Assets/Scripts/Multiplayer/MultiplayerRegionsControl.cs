using System;
using System.Collections.Generic;
using Fusion;

namespace Multiplayer
{
    public class MultiplayerRegionsControl : NetworkBehaviour
    {
        public static MultiplayerRegionsControl instance { get; private set; }

        internal List<MultiplayerGameRegionControl> multigameRegionControls;
        
        private void Start()
        {
            multigameRegionControls = new List<MultiplayerGameRegionControl>();
            instance = this;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_InitialRoom(int roomIndex, int[] enemiesIndex, int[] enemiesSpawnIndex)
        {
            multigameRegionControls[roomIndex].RPC_InitialRoom(enemiesIndex, enemiesSpawnIndex);
        }
    }
}