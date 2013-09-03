﻿using strange.extensions.injector.impl;
using strange.extensions.injector.api;
using strange.framework.api;
using System;

public class CrossContextInjectionBinder : InjectionBinder, ICrossContextInjectionBinder
{
    /// Cross Context Injector is shared with all child contexts.
    public IInjectionBinder CrossContextBinder { get; set; }

    public CrossContextInjectionBinder() : base()
    {
    }
    public override IInjectionBinding GetBinding<T>()
    {
        return GetBinding(typeof(T), null);
    }

    public override IInjectionBinding GetBinding(object key, object name)
    {
        
        IInjectionBinding binding = base.GetBinding(key, name) as IInjectionBinding;

        if (CrossContextBinder != null)
        {

            if (binding == null) //Attempt to get this from the cross context. Cross context is always SECOND PRIORITY. Local injections always override
            {
                if (CrossContextBinder != null)
                {
                    binding = CrossContextBinder.GetBinding(key, name) as IInjectionBinding;
                }
                else
                {
                    throw new InjectionException("Cross Context Injector is null while attempting to resolve a cross context binding", InjectionExceptionType.MISSING_CROSS_CONTEXT_INJECTOR);
                }
            }
        }

        return binding;
    }

    protected void AddBinding(InjectionBinding binding)
    {

    }

    override public void resolveBinding(IBinding binding, object key)
    {
        //Decide whether to resolve locally or not
        if (binding is IInjectionBinding)
        {
            InjectionBinding injectionBinding = (InjectionBinding)binding;
            if (injectionBinding.isCrossContext)
            {

                if (CrossContextBinder == null) //We are a crosscontextbinder
                {
                    base.resolveBinding(binding, key);
                }
                else 
                {
                    CrossContextBinder.resolveBinding(binding, key);
                }
            }
            else
            {
                base.resolveBinding(binding, key);
            }
        }
       

    }
}