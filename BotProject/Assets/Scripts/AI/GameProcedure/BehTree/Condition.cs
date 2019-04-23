namespace GameProcedure.BehTree
{
    using GameUtils;
    using GameUtils.ObjectPool;

    public abstract class Condition
    {
        public abstract bool IsTrue(WorkData data);
    }

    public class ORCondition : Condition, IPoolable
    {
        private Condition lhsCondition;
        private Condition rhsCondition;

        public ORCondition() { }

        public static Condition Create(Condition lhs, Condition rhs)
        {
            var retMe = Pool<ORCondition>.Allocate();
            retMe.lhsCondition = lhs;
            retMe.rhsCondition = rhs;
            return retMe;
        }

        public override bool IsTrue(WorkData data)
        {
            return lhsCondition.IsTrue(data) || rhsCondition.IsTrue(data);
        }

        public void Recycle()
        {
            lhsCondition = rhsCondition = null;
        }
    }

    public class ANDCondition : Condition, IPoolable
    {
        private Condition lhsCondition;
        private Condition rhsCondition;

        public ANDCondition() { }

        public static Condition Create(Condition lhs, Condition rhs)
        {
            var retMe = Pool<ANDCondition>.Allocate();
            retMe.lhsCondition = lhs;
            retMe.rhsCondition = rhs;
            return retMe;
        }

        public override bool IsTrue(WorkData data)
        {
            return lhsCondition.IsTrue(data) && rhsCondition.IsTrue(data);
        }

        public void Recycle()
        {
            lhsCondition = rhsCondition = null;
        }
    }

    public class XORCondition : Condition, IPoolable
    {
        private Condition lhsCondition;
        private Condition rhsCondition;

        public XORCondition() { }

        public static Condition Create(Condition lhs, Condition rhs)
        {
            var retMe = Pool<XORCondition>.Allocate();
            retMe.lhsCondition = lhs;
            retMe.rhsCondition = rhs;
            return retMe;
        }

        public override bool IsTrue(WorkData data)
        {
            return lhsCondition.IsTrue(data) ^ rhsCondition.IsTrue(data);
        }

        public void Recycle()
        {
            lhsCondition = rhsCondition = null;
        }
    }
}