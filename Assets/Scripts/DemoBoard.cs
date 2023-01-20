using Crosswork.Core;
using Crosswork.View;
using System.Collections;
using UnityEngine;

namespace Crosswork.Demo
{
    public abstract class DemoBoardSystem
    {
        public virtual void Load(CrossworkBoard board, CrossworkBoardView view, DemoBoardInput input)
        { }
        public virtual void Unload()
        { }
        public virtual void Update(float delta)
        { }
    }

    public class DemoLevelManifest
    {
        public DemoLevelModel Model;
        public DemoBoardSystem[] Systems;
    }

    public class DemoBoard : MonoBehaviour
    {
        [SerializeField]
        private DemoBoardConfig config;
        [SerializeField]
        private CrossworkBoardView view;
        [SerializeField]
        private DemoBoardInput input;

        private CrossworkBoard board;
        private DemoBoardSystem[] systems;

        private void Awake()
        {
            view.Initialize(config);
            board = new CrossworkBoard(view, config);
        }

        public void Load(DemoLevelManifest level)
        {
            board.Load(level.Model);

            systems = level.Systems;
            for (int i = 0; i < systems.Length; ++i)
            {
                systems[i].Load(board, view, input);
            }
        }

        public void Unload()
        {
            for (int i = 0; i < systems.Length; ++i)
            {
                systems[i].Unload();
            }

            systems = null;
            board.Unload();
        }

        private void Update()
        {
            if (!board.Loaded)
            {
                return;
            }

            for (int i = 0; i < systems.Length; ++i)
            {
                systems[i].Update(Time.deltaTime);
            }
        }
    }
}