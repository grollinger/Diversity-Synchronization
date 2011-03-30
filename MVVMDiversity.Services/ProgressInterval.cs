﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVMDiversity.Model;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;

namespace MVVMDiversity.Services
{
    class ProgressInterval : IAdvanceProgress
    {
        BackgroundOperation _progress;
        float _deltaPerStep = 0f;
        int _initialProgress = 0;
        int _currentProgress = 0;
        int _steps = 1;
        int _stepsPerIncrement = 1;
        int _deltaSteps = 0;

        public ProgressInterval(BackgroundOperation p, float delta , int steps)
        {
            if(p==null)
                throw new ArgumentNullException();

            _progress = p;
            _initialProgress = p.Progress;
            calculateDeltas(delta, steps);
            
        }

        private void calculateDeltas(float delta, int steps)
        {
            _deltaPerStep = delta / steps;

            _stepsPerIncrement = (int)Math.Ceiling(1f / _deltaPerStep);
        }

        public void advance()
        {
            _deltaSteps++;
            pushIncrements();

        }

        private void pushIncrements()
        {
            if (_deltaSteps > _stepsPerIncrement)
            {
                var remainder = _deltaSteps % _stepsPerIncrement;
                _currentProgress += _deltaSteps - remainder;
                _deltaSteps = remainder;
                
                _progress.Progress = _initialProgress + (int)(_deltaPerStep * _currentProgress);
            }
        }

        public int InternalTotal
        {
            get
            {
                return _steps;
            }
            set
            {
                _steps = value;

            }
        }

        public bool IsCancelRequested
        {
            get { return _progress.IsCancelRequested; }
        }

        public void advance(int steps)
        {
            _deltaSteps += steps;
            pushIncrements();
        }
    }
}