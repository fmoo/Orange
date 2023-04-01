using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ActionQueue : MonoBehaviour {
    class Item {
        public Action OnDone = null;
        public bool IsDone() {
            return isDone;
        }

        Action immediate = null;
        IEnumerator asyncWork = null;
        bool isDone = false;
        public Coroutine Start(MonoBehaviour executor) {
            if (immediate != null) {
                immediate();
                isDone = true;
                OnDone?.Invoke();
                return null;
            } else if (asyncWork != null) {
                return executor.StartCoroutine(asyncWork);
            } else {
                Debug.LogError("Error making work!", executor);
                isDone = true;
                OnDone?.Invoke();
                return null;
            }
        }

        IEnumerator Wrap(Action<Action> work) {
            Action onDone = () => {
                isDone = true;
                OnDone?.Invoke();
            };
            work(onDone);
            while (!isDone) {
                // TODO: Might be nice to specify a time other than "every frame"
                yield return null;
            }
        }
        IEnumerator Wrap(IEnumerator work) {
            yield return work;
            isDone = true;
            OnDone?.Invoke();
        }

        IEnumerator Wrap(Coroutine work) {
            yield return work;
            isDone = true;
            OnDone?.Invoke();
        }

        public Item(Action work, Action onDone = null) {
            immediate = work;
            if (onDone != null) this.OnDone += onDone;
        }
        public Item(Action<Action> work, Action onDone = null) {
            asyncWork = Wrap(work);
            if (onDone != null) this.OnDone += onDone;
        }
        public Item(IEnumerator work, Action onDone = null) {
            asyncWork = Wrap(work);
            if (onDone != null) this.OnDone += onDone;
        }
        public Item(Coroutine work, Action onDone = null) {
            asyncWork = Wrap(work);
            if (onDone != null) this.OnDone += onDone;
        }
    }

    Coroutine activeCoroutine = null;
    Item activeItem = null;
    Queue<Item> inactiveItems = new Queue<Item>();

    public void Enqueue(Action work) {
        inactiveItems.Enqueue(
            new Item(work)
        );
        MaybeProcessItems();
    }

    public void Enqueue(Coroutine work) {
        inactiveItems.Enqueue(
            new Item(work)
        );
        MaybeProcessItems();
    }

    public void Enqueue(Action<Action> work) {
        inactiveItems.Enqueue(
            new Item(work)
        );
        MaybeProcessItems();
    }

    public void Enqueue(IEnumerator work) {
        inactiveItems.Enqueue(
            new Item(work)
        );
        MaybeProcessItems();
    }

    public void Clear() {
        activeItem = null;
        inactiveItems.Clear();
        if (activeCoroutine != null) {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
    }

    // TODO: Wrappers for DOTween? UniRx?


    void OnItemDone() {
        activeCoroutine = null;
        activeItem = null;
        MaybeProcessItems();
    }

    void OnEnable() {
        MaybeProcessItems();
    }

    void MaybeProcessItems() {
        if (!gameObject.activeInHierarchy) return;
        if (activeItem != null || inactiveItems.IsNullOrEmpty()) return;

        activeItem = inactiveItems.Dequeue();
        activeCoroutine = activeItem.Start(this);
        while (activeItem.IsDone()) {
            if (inactiveItems.IsNullOrEmpty()) {
                activeCoroutine = null;
                activeItem = null;
                return;
            }
            activeItem = inactiveItems.Dequeue();
            activeCoroutine = activeItem.Start(this);
        }
        activeItem.OnDone += OnItemDone;
    }

}