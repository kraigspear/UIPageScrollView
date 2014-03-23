using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace Spearware.iOS
{
    public class ConstraintBuilder
    {
        readonly List<NSLayoutConstraint> _constraints = new List<NSLayoutConstraint>();

		readonly UIView _parentView;
		readonly UIView[] _children;
		readonly UIView _child;

        public ConstraintBuilder(UIView parentView, UIView child) : this(parentView, new [] {child})
        {

        }

        public ConstraintBuilder(UIView parentView, UIView[] children)
        {
            if (children == null)
                throw new ArgumentNullException("children", "child");
            if (parentView == null)
                throw new ArgumentNullException("parentView");

            if (children.Length < 1)
                throw new ArgumentNullException("children", "no children");

            foreach (UIView view in children)
            {
                if (!view.TranslatesAutoresizingMaskIntoConstraints)
                    continue;
                view.TranslatesAutoresizingMaskIntoConstraints = false;
            }

            _child = children[0];
            _children = children;
            _parentView = parentView;
        }

        /// <summary>
        /// Set the left position
        /// </summary>
        /// <returns>The left.</returns>
        /// <param name="distance">Distance.</param>
        public ConstraintBuilder WithLeft(float distance = 0)
        {
            var constraint = NSLayoutConstraint.Create(_child, NSLayoutAttribute.Left, NSLayoutRelation.Equal, _parentView, NSLayoutAttribute.Left, 1, distance);
            _constraints.Add(constraint);
            return this;
        }

        public ConstraintBuilder WithRight(float distance = 0)
        {
            var constraint = NSLayoutConstraint.Create(_child, NSLayoutAttribute.Right, NSLayoutRelation.Equal, _parentView, NSLayoutAttribute.Right, 1, distance);
            _constraints.Add(constraint);
            return this;
        }


        /// <summary>
        /// Set the left position
        /// </summary>
        /// <returns>The left.</returns>
        /// <param name="distance">Distance.</param>
		/// <param name = "ofView">View to be at the left of</param>
        public ConstraintBuilder WithLeftOfView(float distance, UIView ofView)
        {
			var constraint = NSLayoutConstraint.Create(_child, NSLayoutAttribute.Right, NSLayoutRelation.Equal, ofView, NSLayoutAttribute.Left, 1, distance);
            _constraints.Add(constraint);
            return this;
        }

        public ConstraintBuilder WithRightOfView(float distance, UIView ofView)
        {
            var constraint = NSLayoutConstraint.Create(_child, NSLayoutAttribute.Left, NSLayoutRelation.Equal, ofView, NSLayoutAttribute.Right, 1, distance);
            _constraints.Add(constraint);
            return this;
        }

        public ConstraintBuilder WithTopOfView(float distance, UIView ofView)
        {
            var constraint = NSLayoutConstraint.Create(_child, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ofView, NSLayoutAttribute.Top, 1, distance);
            _constraints.Add(constraint);
            return this;
        }

        public ConstraintBuilder WithBottomOfView(float distance, UIView ofView)
        {
            var constraint = NSLayoutConstraint.Create(_child, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ofView, NSLayoutAttribute.Bottom, 1, distance);
            _constraints.Add(constraint);
            return this;
        }

        public ConstraintBuilder WithUnderOfView(float distance, NSObject ofView)
        {
            var constraint = NSLayoutConstraint.Create(_child, NSLayoutAttribute.Top, NSLayoutRelation.Equal, ofView, NSLayoutAttribute.Bottom, 1, distance);
            _constraints.Add(constraint);
            return this;
        }

        public ConstraintBuilder WithAboveOfView(float distance, NSObject ofView)
        {
            var constraint = NSLayoutConstraint.Create(_child, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, ofView, NSLayoutAttribute.Top, 1, distance);
            _constraints.Add(constraint);
            return this;
        }

        /// <summary>
        /// Center Y of child on parent
        /// </summary>
        /// <returns>The center y.</returns>
        public ConstraintBuilder WithCenterY(float multiplier = 1, float constant = 0)
        {
            if (_children.Length > 1)
            {
                WithCenterYMultiple();
            }
            else
            {
                var constraint = NSLayoutConstraint.Create(_child, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, _parentView, NSLayoutAttribute.CenterY, multiplier, constant);
                _constraints.Add(constraint);
            }
            return this;
        }

        public ConstraintBuilder WithCenterX(float multiplier = 1)
        {

            var constraint = NSLayoutConstraint.Create(_child, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, _parentView, NSLayoutAttribute.CenterX, multiplier, 0);
            _constraints.Add(constraint);

            return this;
        }

        public ConstraintBuilder WithHeight(float height)
        {
            _constraints.Add(NSLayoutConstraint.Create(_child, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, height));
            return this;
        }

        public ConstraintBuilder WithWidth(float width)
        {
            _constraints.Add(NSLayoutConstraint.Create(_child, NSLayoutAttribute.Width, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, width));
            return this;
        }

        public ConstraintBuilder WithWidth(NSObject ofThisObject, float multiplier = 1)
        {
            _constraints.Add(NSLayoutConstraint.Create(_child, NSLayoutAttribute.Width, NSLayoutRelation.Equal, ofThisObject, NSLayoutAttribute.Width, multiplier, 0));
            return this;
        }

        public ConstraintBuilder WithHeight(NSObject ofThisObject)
        {
            _constraints.Add(NSLayoutConstraint.Create(_child, NSLayoutAttribute.Height, NSLayoutRelation.Equal, ofThisObject, NSLayoutAttribute.Height, 1, 0));
            return this;
        }

        public ConstraintBuilder WithParentTop(float distance = 0)
        {
            _constraints.Add(NSLayoutConstraint.Create(_child, NSLayoutAttribute.Top, NSLayoutRelation.Equal, _parentView, NSLayoutAttribute.Top, 1, distance));
            return this;
        }
//
//        public ConstraintBuilder WithUnderNavBar(UIViewController viewController)
//        {
//
//            var layoutSupport = new UILayoutSupport();
//            //var layoutGuide = new UILayoutSupport(viewController.TopLayoutGuide.Handle);
//
//            NSDictionary viewDictionary = NSDictionary.FromObjectsAndKeys(new NSObject[] { _child, layoutSupport }, new NSObject[] { (NSString)"_child", (NSString) "layoutGuide" });
//            var constraints = NSLayoutConstraint.FromVisualFormat("V:[layoutGuide]-0-[_child]", 0, null, viewDictionary);
//
//            _constraints.AddRange(constraints);
//
//            return this;
//        }

        public ConstraintBuilder WithParentBottom(float distance = 1.0f)
        {
            _constraints.Add(NSLayoutConstraint.Create(_child, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, _parentView, NSLayoutAttribute.Bottom, 1, distance));
            return this;
        }

        public ConstraintBuilder WithTopAllignedToParentBottom(float distance = 1.0f)
        {
            _constraints.Add(NSLayoutConstraint.Create(_child, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, _parentView, NSLayoutAttribute.Top, 1, distance));
            return this;
        }

        public ConstraintBuilder WithParentRight(float distance = 1.0f)
        {
            _constraints.Add(NSLayoutConstraint.Create(_child, NSLayoutAttribute.Right, NSLayoutRelation.Equal, _parentView, NSLayoutAttribute.Right, 1, distance));
            return this;
        }

        public ConstraintBuilder WithParentLeft(float distance = 1.0f)
        {
            _constraints.Add(NSLayoutConstraint.Create(_child, NSLayoutAttribute.Left, NSLayoutRelation.Equal, _parentView, NSLayoutAttribute.Right, 1, distance));
            return this;
        }

        public ConstraintBuilder WithParentHeight(float multiplier = 1.0f)
        {
            _constraints.Add(NSLayoutConstraint.Create(_child, NSLayoutAttribute.Height, NSLayoutRelation.Equal, _parentView, NSLayoutAttribute.Height, multiplier, 0));
            return this;
        }

        public ConstraintBuilder WithCenterYOfView(UIView otherView)
        {
            _constraints.Add(
                NSLayoutConstraint.Create(_child, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, otherView, NSLayoutAttribute.CenterY, 1, 0));
            return this;
        }

        /// <summary>
        /// Equal space all of views on its parent.
        /// </summary>
        /// <returns>The y.</returns>
        void WithCenterYMultiple()
        {
            var firstChild = _children[0];

            float subtract = -(_children.Length * 25);
            _constraints.Add(NSLayoutConstraint.Create(firstChild, 
                                                       NSLayoutAttribute.CenterY, 
                                                       NSLayoutRelation.Equal,
                                                       _parentView, 
                                                       NSLayoutAttribute.CenterY, 
                                                       1, 
                                                       subtract));

            for (int i = 1; i < _children.Length; i++)
            {
                var child = _children[i];
                var constraint = NSLayoutConstraint.Create(child, NSLayoutAttribute.Top, NSLayoutRelation.Equal, firstChild, NSLayoutAttribute.Bottom, 1, 5);
                _constraints.Add(constraint);
            }

        }

        /// <summary>
        /// Apply the added constraints
        /// </summary>
        public IEnumerable<NSLayoutConstraint> Apply()
        {
            _parentView.AddConstraints(_constraints.ToArray());

            return _constraints.AsReadOnly();
        }

        public IEnumerable<NSLayoutConstraint> Constraints
        {
            get
            {
                return _constraints.AsReadOnly();
            }
        }
    }
}

