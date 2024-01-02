// Copyright (C) 2015 Jaroslav Stehlik - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEditor;
using UnityEngine;


namespace SVGImporter
{

    public interface IModal
    {

        void ModalRequest(bool shift);

        void ModalClosed(ModalWindow window);
    }

    public enum WindowResult
    {
        None,
        Ok,
        Cancel,
        Invalid,
        LostFocus
    }

    public abstract class ModalWindow : EditorWindow
    {
        public const float TITLEBAR = 18;
        
        protected IModal owner;
        
        protected WindowResult result = WindowResult.None;
        
        public WindowResult Result
        {
            get { return result; }
        }
        
        protected virtual void OnLostFocus()
        {
            result = WindowResult.LostFocus;
            
            if (owner != null)
                owner.ModalClosed(this);
        }
        
        protected virtual void Cancel()
        {
            result = WindowResult.Cancel;
            
            if (owner != null)
                owner.ModalClosed(this);
            
            Close();
        }
        
        protected virtual void Ok()
        {
            result = WindowResult.Ok;
            
            if (owner != null)
                owner.ModalClosed(this);
            
            Close();
        }

        protected abstract void Draw(Rect region);
    }
}
