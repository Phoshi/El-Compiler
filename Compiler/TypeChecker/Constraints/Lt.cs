﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedycloud.Compiler.TypeChecker.Constraints {
    public class Lt : ITypeConstraint {
        public int Num { get; private set; }

        public Lt(int num) {
            Num = num;
        }

        public bool Equals(ITypeConstraint constraint) {
            if (constraint is Lt) {
                return Num == ((Lt) constraint).Num;
            }
            return false;
        }

        public bool IsAssignableTo(ITypeConstraint constraint) {
            if (constraint is Lt) {
                return Num <= ((Lt) constraint).Num;
            }
            return false;
        }

        public bool IsSupertypeOf(ITypeConstraint constraint) {
            if (constraint is Lt) {
                return Num > ((Lt)constraint).Num;
            }
            return false;
        }

        public bool IsSubtypeOf(ITypeConstraint constraint) {
            if (constraint is Lt) {
                return Num < ((Lt)constraint).Num;
            }
            return false;
        }
    }
}