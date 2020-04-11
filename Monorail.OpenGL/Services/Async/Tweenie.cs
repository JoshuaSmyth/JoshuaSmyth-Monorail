using Monorail.Framework.Services.ServiceLocation;
using Monorail.Framework.Services.UserInterface;
using System;
using System.Collections;

namespace Monorail.Framework.Services.Async
{
    public class Tweenie : ILocatableService
    {
        public Continuation TweenElementHorizontaly(IElement element, float destination, float timeMs) {
            var continuation = new Continuation(LinearTweenHorizontally(element, destination, timeMs));
            return continuation;
        }

        public Continuation TweenElementVertically(IElement element, float destination, float timeMs) {
            var continuation = new Continuation(LinearTweenVertically(element, destination, timeMs));
            return continuation;
        }

        private IEnumerator LinearTweenVertically(IElement element, float destination, float timeMs) {

            var n = Math.Sign(destination - element.OffsetY);
            var d = Math.Abs(destination - element.OffsetY);
            var perMs = d / timeMs;
            while (d > 0.01)
            {
                var delta = n*perMs*Coroutines.DeltaMilliseconds;
                element.OffsetY += delta;
                d-=n*delta;

                if (d > 0.01) {
                    yield return null;
                }
            }

            element.OffsetY = destination;
        }

        private IEnumerator LinearTweenHorizontally(IElement element, float destination, float timeMs) {

            var n = Math.Sign(destination - element.OffsetX);
            var d = Math.Abs(destination - element.OffsetX);
            var perMs = d / timeMs;
            while (d > 0.01)
            {
                var delta = n*perMs*Coroutines.DeltaMilliseconds;
                element.OffsetX += delta;
                d-=n*delta;

                if (d > 0.01) {
                    yield return null;
                }
            }

            element.OffsetX = destination;
        }

        public void Init(params object[] objects) {
            
        }

        public void ResolveDependancies(IServiceMapper serviceMapper) {

        }
    }
}
