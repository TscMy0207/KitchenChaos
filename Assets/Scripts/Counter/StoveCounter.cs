using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

/// <summary>
/// ZZ
/// </summary>
public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> onProgressChanged;

    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;

    private State state;
    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSO;
    private FryingRecipeSO burnedRecipeSO;

    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                ////ZZ XHU5OGRmXHU3MjY5XHU3MGY5XHU5OTZhXHU2NzA5XHU5NWVlXHU5ODk4XHVmZjBjXHU3MTZlXHU5OGRmXHU3MjY5XHU3Njg0XHU2NWY2XHU5NWY0XHU2Y2ExXHU2NzA5XHU0ZmRkXHU1YjU4XHVmZjBjXHU2MmZmXHU0ZTBiXHU3MTZlXHU1OTdkXHU3Njg0XHU5OGRmXHU3MjY5XHVmZjBjXHU1YjgzXHU3Njg0XHU3MmI2XHU2MDAxXHU1M2M4XHU0ZWNlXHU3NTFmXHU4MDg5XHU1ZjAwXHU1OWNi
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    onProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    });
                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                        state = State.Fried;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                    }
                    break;

                case State.Fried:
                    burningTimer += Time.deltaTime;
                    onProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                    {
                        progressNormalized = burningTimer / burnedRecipeSO.fryingTimerMax
                    });
                    if (burningTimer > burnedRecipeSO.fryingTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burnedRecipeSO.output, this);
                        state = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                        onProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                        {
                            progressNormalized = 0f
                        });
                    }
                    break;
                ////ZZ
                case State.Burned:
                    break;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());
                    burnedRecipeSO = GetFryingRecipeSOWithInput(fryingRecipeSO.output);
                    state = State.Frying;
                    fryingTimer = 0;
                    burningTimer = 0;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                    {
                        state = state
                    });
                    onProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    });
                }
            }
            else
            {
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        state = State.Idle;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                        onProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                        {
                            progressNormalized = 0
                        });
                    }
                }
            }
            else
            {
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });
                onProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs()
                {
                    progressNormalized = 0
                });
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjectSO GetOutPutForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }
}