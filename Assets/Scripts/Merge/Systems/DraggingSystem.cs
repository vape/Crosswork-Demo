using Crosswork.Core;
using Crosswork.Demo.Merge.Elements;
using Crosswork.Demo.Tweens;
using Crosswork.View;
using Crosswork.View.Sorting;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crosswork.Demo.Merge.Systems
{
    public class DraggingSystem : DemoBoardSystem
    {
        private MergingSystem merging;
        private CrossworkBoard board;
        private DemoBoardInput input;
        private CrossworkBoardView view;

        private Vector3 draggingElementLastPosition;
        private MergeChip draggingElement;
        private ElementLockKey draggingElementLock;

        public DraggingSystem(MergingSystem merging)
        {
            this.merging = merging;
        }

        public override void Load(CrossworkBoard board, CrossworkBoardView view, DemoBoardInput input)
        {
            base.Load(board, view, input);

            this.board = board;
            this.input = input;
            this.view = view;

            input.Input += InputHandler;
        }

        public override void Unload()
        {
            base.Unload();

            input.Input -= InputHandler;
        }

        private void InputHandler(InputEventType type, PointerEventData data)
        {
            switch (type)
            {
                case InputEventType.BeginDrag:
                    OnBeginDrag(data);
                    return;

                case InputEventType.Drag:
                    OnDrag(data);
                    return;

                case InputEventType.EndDrag:
                    OnEndDrag(data);
                    return;
            }
        }

        private void OnBeginDrag(PointerEventData data)
        {
            if (draggingElement != null)
            {
                return;
            }

            var position = view.ScreenToGridPosition(data.position);
            if (!board.InsideBounds(position))
            {
                return;
            }

            ref var bucket = ref board.GetBucketAt(position);
            for (int i = 0; i < bucket.Count; ++i)
            {
                var element = bucket.Elements[i] as MergeChip;
                if (element == null)
                {
                    continue;
                }

                if (board.IsElementLocked(element))
                {
                    return;
                }

                draggingElement = element;
                draggingElementLock = board.LockElement(draggingElement);

                if (view.TryGetView(draggingElement, out var elementView))
                {
                    CrossworkSortingUtility.SetLayer(elementView, MergeLayers.DraggingElement);
                }
            }
        }

        private void OnDrag(PointerEventData data)
        {
            if (draggingElement == null)
            {
                return;
            }

            if (view.TryGetView(draggingElement, out var draggingElementView))
            {
                draggingElementView.transform.position = view.ScreenToWorldPosition(data.position);
            }
        }

        private void OnEndDrag(PointerEventData data)
        {
            if (draggingElement == null)
            {
                return;
            }

            if (view.TryGetView(draggingElement, out var draggingElementView))
            {
                draggingElementLastPosition = draggingElementView.transform.position;
                CrossworkSortingUtility.ResetLayer(draggingElementView);
            }

            board.UnlockElement(draggingElementLock);
            draggingElementLock = default;

            var targetPosition = view.ScreenToGridPosition(data.position);
            if (!board.InsideBounds(targetPosition))
            {
                OnDragFailed();
                return;
            }

            ref var bucket = ref board.GetBucketAt(targetPosition);
            for (int i = 0; i < bucket.Count; ++i)
            {
                var element = bucket.Elements[i] as MergeChip;
                if (element == null)
                {
                    continue;
                }

                if (board.IsElementLocked(element))
                {
                    OnDragFailed();
                    return;
                }

                board.UnlockElement(draggingElementLock);

                if (!merging.TryMerge(draggingElement, element))
                {
                    OnDragFailed(element);
                    return;
                }
                else
                {
                    OnDragSuccess();
                    return;
                }
            }

            if (board.TryMoveElement(draggingElement, targetPosition.x, targetPosition.y))
            {
                OnDragMoved();
            }
            else
            {
                OnDragFailed();
            }
        }

        private void OnDragMoved()
        {
            view.StartCoroutine(ResetPositionRoutine(draggingElementLastPosition, draggingElement));
            draggingElement = null;
        }

        private void OnDragFailed(Element elementToSwap = null)
        {
            if (elementToSwap == null || !TrySwap(draggingElement, elementToSwap))
            {
                view.StartCoroutine(ResetPositionRoutine(draggingElement));
            }
            
            draggingElement = null;
        }

        private void OnDragSuccess()
        {
            draggingElement = null;
        }

        private bool TrySwap(Element element0, Element element1)
        {
            var view0 = view.GetView(element0);
            var view1 = view.GetView(element1);

            var pos0 = view0.transform.position;
            var pos1 = view1.transform.position;

            if (board.TrySwapElements(element0, element1))
            {
                view.StartCoroutine(ResetPositionRoutine(pos0, element0));
                view.StartCoroutine(ResetPositionRoutine(pos1, element1));

                return true;
            }

            return false;
        }

        private IEnumerator ResetPositionRoutine(Vector3 from, Element element)
        {
            view.GetView(element).transform.position = from;

            yield return ResetPositionRoutine(element);
        }

        private IEnumerator ResetPositionRoutine(Element element)
        {
            var view = this.view.GetView(element);
            var lockKey = board.LockElement(element);

            var targetPosition = this.view.GridToWorldPosition(element.GetCell().Position);
            view.transform.TweenMove(targetPosition, 0.15f).Play();

            yield return new WaitForSeconds(0.15f);

            this.view.ResetPosition(element);

            board.UnlockElement(lockKey);
        }
    }
}
