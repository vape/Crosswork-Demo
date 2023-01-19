using Crosswork.Core;
using Crosswork.Demo.Match3.Systems.Matching;
using Crosswork.Demo.Tweens;
using Crosswork.View;
using Crosswork.View.Sorting;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crosswork.Demo.Match3.Systems
{
    public class SwappingSystem : DemoBoardSystem
    {
        private enum SwapDirection
        {
            Left,
            Up,
            Right,
            Down
        }

        const float SwapThreshold = 0.25f;

        protected MatchingSystem matching;

        protected CrossworkBoard board;
        protected DemoBoardInput input;
        protected CrossworkBoardView view;

        private bool isDragging;
        private Vector2Int elementPosition;
        private Vector2 dragStartPosition;

        public SwappingSystem(MatchingSystem matching)
        {
            this.matching = matching;
        }

        public override void Load(CrossworkBoard board, CrossworkBoardView view, DemoBoardInput input)
        {
            base.Load(board, view, input);

            this.board = board;
            this.view = view;
            this.input = input;

            this.input.Input += InputHandler;
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
                    break;
                case InputEventType.Drag:
                    OnDrag(data);
                    break;
                case InputEventType.EndDrag:
                    OnEndDrag(data);
                    break;
            }
        }

        private void OnEndDrag(PointerEventData data)
        {
            if (!isDragging)
            {
                return;
            }

            isDragging = false;
        }

        private void OnBeginDrag(PointerEventData data)
        {
            if (isDragging)
            {
                return;
            }

            dragStartPosition = data.position;
            elementPosition = view.ScreenToGridPosition(data.position);
            isDragging = true;
        }

        private void OnDrag(PointerEventData data)
        {
            if (!isDragging)
            {
                return;
            }

            var startDragLP = view.ScreenToLocalPosition(dragStartPosition);
            var currentDragLP = view.ScreenToLocalPosition(data.position);

            var delta = currentDragLP - startDragLP;
            if (delta.magnitude >= SwapThreshold)
            {
                var direction = GetDirection(delta);
                var position = elementPosition + direction;
                var current = board.GetElementsAt(elementPosition.x, elementPosition.y).FirstOrDefault();
                var next = board.GetElementsAt(position.x, position.y).FirstOrDefault();

                if (current != null && next != null && board.CanSwapElements(current, next))
                {
                    view.StartCoroutine(SwapRoutine(current, next, checkMatch: true));
                }

                isDragging = false;
            }
        }

        private IEnumerator SwapRoutine(Element current, Element next, bool checkMatch)
        {
            const float duration = 0.2f;

            var currentLock = board.LockElement(current);
            var nextLock = board.LockElement(next);

            if (view.TryGetView(current, out var currentView) && view.TryGetView(next, out var nextView))
            {
                CrossworkSortingUtility.SetLayer(currentView, Match3Layers.SwapTopLayer);

                currentView.transform.TweenLocalMove(nextView.transform.position, duration).Play();
                nextView.transform.TweenLocalMove(currentView.transform.position, duration).Play();

                TweenSequence.Create()
                    .Add(currentView.transform.TweenScale(new Vector3(1.4f, 1.4f), duration * 0.5f).SetEase(TweenEasing.OutQuad))
                    .Add(currentView.transform.TweenScale(new Vector3(1.00f, 1.00f), duration * 0.5f).SetEase(TweenEasing.InQuad))
                    .Play();

                TweenSequence.Create()
                    .Add(nextView.transform.TweenScale(new Vector3(0.6f, 0.6f), duration * 0.5f).SetEase(TweenEasing.OutQuad))
                    .Add(nextView.transform.TweenScale(new Vector3(1.00f, 1.00f), duration * 0.5f).SetEase(TweenEasing.InQuad))
                    .Play();
            }

            yield return new WaitForSeconds(duration);

            CrossworkSortingUtility.ResetLayer(currentView);

            board.UnlockElement(currentLock);
            board.UnlockElement(nextLock);

            board.SwapElements(current, next);

            if (checkMatch)
            {
                matching.ForceUpdate();

                if (!matching.HasMatchAt(current.GetCell().Position) && !matching.HasMatchAt(next.GetCell().Position))
                {
                    yield return SwapRoutine(next, current, checkMatch: false);
                }
            }
        }

        private Vector2Int GetDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x > 0 ? Vector2Int.right : Vector2Int.left;
            }
            else
            {
                return delta.y > 0 ? Vector2Int.up : Vector2Int.down;
            }
        }
    }
}
