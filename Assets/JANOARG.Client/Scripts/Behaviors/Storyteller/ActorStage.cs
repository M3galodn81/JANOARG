using System;
using System.Collections;
using System.Collections.Generic;
using JANOARG.Client.Data.Constant;
using JANOARG.Shared.Utils.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace JANOARG.Client.Behaviors.Storyteller
{
    public class StoryActorStage : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _StageRoot;
        [SerializeField] public CanvasGroup CanvasGroup;
        [SerializeField] private Image _ActorPrefab;
        [SerializeField] private StoryActorConstants _ActorConstants;

        [Header("Position Markers")]
        [SerializeField] private RectTransform _LeftPoint;
        [SerializeField] private RectTransform _MiddlePoint;
        [SerializeField] private RectTransform _RightPoint;
        [SerializeField] private RectTransform _OutLeftPoint;
        [SerializeField] private RectTransform _OutRightPoint;

        private readonly Dictionary<string, Image> _Actors = new();
        private readonly Dictionary<string, Coroutine> _Movements = new();

        private RectTransform GetPoint(ActorPosition position)
        {
            return position switch
            {
                ActorPosition.Left => _LeftPoint,
                ActorPosition.Middle => _MiddlePoint,
                ActorPosition.Right => _RightPoint,
                ActorPosition.OutLeft => _OutLeftPoint,
                ActorPosition.OutRight => _OutRightPoint,
                _ => null
            };
        }

        private Vector2 GetPosition(ActorPosition position)
        {
            RectTransform point = GetPoint(position);

            if (point == null)
                throw new InvalidOperationException(
                    $"No marker assigned for {position}");

            // Convert the marker's world position into stage-local coordinates.
            return _StageRoot.InverseTransformPoint(point.position);
        }

        private Image CreateActor(string alias)
        {
            Image image = Instantiate(_ActorPrefab, _StageRoot);
            image.name = $"Actor_{alias}";
            image.preserveAspect = true;
            image.gameObject.SetActive(true);

            _Actors.Add(alias, image);
            return image;
        }

        public IEnumerator ShowActor(
            string alias,
            string spriteID,
            ActorPosition position,
            bool bringToFront = true)
        {
            ActorInfo actor = _ActorConstants.GetActor(alias);

            if (actor == null)
            {
                Debug.LogWarning($"Actor not found: {alias}");
                yield break;
            }

            ActorSprite sprite = Array.Find(actor.Sprites, s => s.ID == spriteID);

            if (sprite == null || sprite.Sprite == null)
            {
                Debug.LogWarning(
                    $"Sprite '{spriteID}' not found for '{alias}'");
                yield break;
            }

            if (!_Actors.TryGetValue(alias, out Image image))
            {
                image = CreateActor(alias);
                image.color = new Color(0f,0f,0f,0f);   
            }

            StopMovement(alias);

            image.sprite = sprite.Sprite;
            image.enabled = true;
            image.rectTransform.anchoredPosition = GetPosition(position);

            if (bringToFront)
                image.transform.SetAsLastSibling();

            if (position != ActorPosition.OutLeft && position != ActorPosition.OutRight)
            {
               yield return Ease.Animate(1f, (a) =>
                {
                    float lerp = Ease.Get(a, EaseFunction.Cubic, EaseMode.Out);
                    image.color = new Color(1f, 1f, 1f, lerp);
                }); 
            } else
            {
                image.color = new Color(1f, 1f, 1f, 1f);
            }
            
        }

        public IEnumerator MoveActor(
            string alias,
            ActorPosition destination,
            float duration = 0.5f,
            bool bringToFront = false)
        {
            if (!_Actors.TryGetValue(alias, out Image image))
            {
                Debug.LogWarning($"Actor is not on stage: {alias}");
                yield break;
            }

            StopMovement(alias);

            if (duration <= 0f)
            {
                image.rectTransform.anchoredPosition =
                    GetPosition(destination);

                FinishExit(alias, destination);
                yield return null;
            }

            Coroutine routine = StartCoroutine(
                MoveRoutine(alias, image, destination, duration));

            _Movements[alias] = routine;

            if (bringToFront)
                image.transform.SetAsLastSibling();
        }

        private IEnumerator MoveRoutine(
            string alias,
            Image image,
            ActorPosition destination,
            float duration)
        {
            RectTransform rect = image.rectTransform;
            Vector2 start = rect.anchoredPosition;
            Vector2 end = GetPosition(destination);

            yield return Ease.Animate(duration, (a) =>
                {
                    float lerp = Ease.Get(a, EaseFunction.Cubic, EaseMode.Out);
                    rect.anchoredPosition = Vector2.Lerp(start, end, lerp);
                });

            rect.anchoredPosition = end;
            _Movements.Remove(alias);

            FinishExit(alias, destination);
        }

        private void FinishExit(
            string alias,
            ActorPosition destination)
        {
            if (destination != ActorPosition.OutLeft &&
                destination != ActorPosition.OutRight)
                return;

            if (_Actors.TryGetValue(alias, out Image image))
            {
                _Actors.Remove(alias);
                Destroy(image.gameObject);
            }
        }

        private void StopMovement(string alias)
        {
            if (_Movements.TryGetValue(alias, out Coroutine routine))
            {
                StopCoroutine(routine);
                _Movements.Remove(alias);
            }
        }

        public void HideActor(string alias)
        {
            StopMovement(alias);

            if (_Actors.TryGetValue(alias, out Image image))
            {
                _Actors.Remove(alias);
                Destroy(image.gameObject);
            }
        }

        public void ClearActors()
        {
            foreach (string alias in new List<string>(_Actors.Keys))
                HideActor(alias);
        }

        public void BringToFront(string alias)
        {
            if (_Actors.TryGetValue(alias, out Image image))
                image.transform.SetAsLastSibling();
        }

        public void SendToBack(string alias)
        {
            if (_Actors.TryGetValue(alias, out Image image))
                image.transform.SetAsFirstSibling();
        }
    
        public IEnumerator HideAllActors()
        {
            yield return Ease.Animate(1, (a) =>
            {
                float lerp = Ease.Get(1-a, EaseFunction.Cubic, EaseMode.Out);
                CanvasGroup.alpha = 1 * lerp;
            });
        }

        public IEnumerator ShowAllActors()
        {
            yield return Ease.Animate(1, (a) =>
            {
                float lerp = Ease.Get(a, EaseFunction.Cubic, EaseMode.Out);
                CanvasGroup.alpha = 1 * lerp;
            });
        }
    }
    public enum ActorPosition
    {
        Left,
        Middle,
        Right,
        OutLeft,
        OutRight
    }
}