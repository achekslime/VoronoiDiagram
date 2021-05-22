using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq.Expressions;
using VoronoiDiagramLib;

namespace VoronoiDiagramLib
{
    public class RedBlackLine
    {
        private Arc mRoot;

        // Пустой лист дерева.
        private Arc blank = new Arc();

        public bool IsBlank(Arc curArc)
        {
            return curArc == blank;
        }


        /// <summary>
        /// Установить корень дерева.
        /// </summary>
        /// <param name="curArc"></param>
        public void SetRoot(Arc curArc)
        {
            mRoot = curArc;
            mRoot.color = Arc.Color.BLACK;
        }
        
        /// <summary>
        /// Проверка дерева на пустоту.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return IsBlank(mRoot);
        }


        /// <summary>
        /// Поиск дуги которая находится над точкой point.
        /// </summary>
        /// <param name="curPoint"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public Arc FindArcAbove(Point curPoint, double line)
        {
            Arc curNode = mRoot;
            bool isFound = false;
            while (!isFound)
            {
                double leftBreakPoint = double.MinValue;
                double rightBreakPoint = double.MaxValue;
                if (!IsBlank(curNode.prev))
                    leftBreakPoint = CalculateBreakPoint(curNode.prev.site.basePoint, curNode.site.basePoint, line);
                if (!IsBlank(curNode.next))
                    rightBreakPoint = CalculateBreakPoint(curNode.site.basePoint, curNode.next.site.basePoint, line);
                if (curPoint.x < leftBreakPoint)
                    curNode = curNode.prev;
                else if (curPoint.x > rightBreakPoint)
                    curNode = curNode.next;
                else
                    isFound = true;
            }

            return curNode;
        }


        /// <summary>
        /// Замена X->Y в береговой линии.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Replace(ref Arc x, ref Arc y)
        {
            // 1) Связываем родителя X с Y
            ParentLinkage(ref x, ref y);

            // 2) Связываем Y со всеми ссылками X.
            y.left = x.left;
            y.right = x.right;
            y.prev = x.prev;
            y.next = x.next;
            y.color = x.color;

            // 3) Пересоединяем все существующие связи X на Y.
            if (!IsBlank(y.left))
                y.left.parent = y;
            if (!IsBlank(y.right))
                y.right.parent = y;
            if (!IsBlank(y.prev))
                y.prev.next = y;
            if (!IsBlank(y.next))
                y.next.prev = y;
            x.color = y.color;
        }

        /// <summary>
        /// Связывает родителя дуги X с дугой Y, а дугу Y с родителем X.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void ParentLinkage(ref Arc x, ref Arc y)
        {
            if (IsBlank(x.parent))
                mRoot = y;
            else if (x.parent.left == x)
                x.parent.left = y;
            else
                x.parent.right = y;
            y.parent = x.parent;
        }

        private Arc MaxLeft(ref Arc x)
        {
            return !IsBlank(x.left) ? MaxLeft(ref x.left) : x;
        }

        /// <summary>
        ///  Удаляет дугу X мз береговой линии.
        /// </summary>
        /// <param name="x"></param>
        public void DeleteArc(ref Arc x)
        {
            Arc.Color removeColor = x.color;
            Arc fixupArc;
            if (IsBlank(x.left))
            {
                fixupArc = x.right;
                ParentLinkage(ref x.parent, ref x.right);
            }

            else if (!IsBlank(x.right))
            {
                fixupArc = x.right;
                ParentLinkage(ref x.parent, ref x.right);
            }
            else
            {
                Arc nextArc = MaxLeft(ref x.right);
                removeColor = nextArc.color;
                fixupArc = nextArc.right;
                if (nextArc.parent == x)
                    x.parent = nextArc;
                else
                {
                    ParentLinkage(ref nextArc, ref nextArc.right);
                    nextArc.right = x.right;
                    nextArc.right.parent = nextArc;
                }

                ParentLinkage(ref x, ref nextArc);
                nextArc.left = x.left;
                nextArc.left.parent = nextArc;
                nextArc.color = x.color;
            }

            // Балансировка.
            if (removeColor == Arc.Color.BLACK)
                FixDdeleting(ref fixupArc);

            if (!IsBlank(x.prev))
                x.prev.next = x.next;
            if (!IsBlank(x.next))
                x.next.prev = x.prev;
        }

        public void FixDdeleting(ref Arc curArc)
        {
            Arc x = curArc;
            Arc p = x.parent;
            
            while (x != mRoot && x.color != Arc.Color.RED)
            {
                // Брат справа.
                if (x == p.left)
                {
                    Arc b = p.right;

                    // Брат - красный, предок - черный.
                    if (b.color == Arc.Color.RED)
                    {
                        b.color = Arc.Color.BLACK;
                        p.color = Arc.Color.RED;
                        LeftRotate(ref p);
                        b = p.right;
                    }

                    // Брат - черный, дети B - черные.
                    if (b.left.color == Arc.Color.BLACK && b.right.color == Arc.Color.BLACK)
                    {
                        b.color = Arc.Color.RED;
                        // цикл
                        x = x.parent;
                    }
                    else
                    {
                        // Только правый сын брата - черный
                        if (b.right.color == Arc.Color.BLACK)
                        {
                            b.left.color = Arc.Color.BLACK;
                            b.color = Arc.Color.RED;
                            RightRotate(ref b);
                            b = p.right;
                        }
                    
                        // Только правый сын брата черный, можно сделать левый поворот p
                        b.color = p.color;
                        p.color = Arc.Color.BLACK;
                        b.right.color = Arc.Color.BLACK;
                        LeftRotate(ref p);
                        x = mRoot;
                    }
                }
                // Брат слева.
                else
                {
                    Arc b = p.left;

                    // Брат - красный, предок - черный.
                    if (b.color == Arc.Color.RED)
                    {
                        b.color = Arc.Color.BLACK;
                        p.color = Arc.Color.RED;
                        RightRotate(ref p);
                        b = p.left;
                    }

                    // Брат - черный, дети B - черные.
                    if (b.left.color == Arc.Color.BLACK && b.right.color == Arc.Color.BLACK)
                    {
                        b.color = Arc.Color.RED;
                        // цикл
                        x = p;
                    }
                    else
                    {
                        // Только правый сын брата - черный
                        if (b.left.color == Arc.Color.BLACK)
                        {
                            b.right.color = Arc.Color.BLACK;
                            b.color = Arc.Color.RED;
                            LeftRotate(ref b);
                            b = p.left;
                        }
                    
                        // Только правый сын брата черный, можно сделать левый поворот p
                        b.color = p.color;
                        p.color = Arc.Color.BLACK;
                        b.left.color = Arc.Color.BLACK;
                        RightRotate(ref p);
                        x = mRoot;
                    }
                }
            }

            // В любой непонятной ситуации делаем корень черным.
            x.color = Arc.Color.BLACK;
        }

        /// <summary>
        /// Вставка дуги Y перед дугой X.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void InsertBefore(ref Arc x, ref Arc y)
        {
            if (IsBlank(x.left))
            {
                x.left = y;
                y.parent = x;
            }
            else
            {
                x.prev.right = y;
                y.parent = x.prev.right;
            }

            if (!IsBlank(x.prev))
                x.prev.next = y;
            y.prev = x.prev;
            x.prev = y;
            y.next = x;

            // Балансируем дерево.
            FixInserting(ref y);
        }

        /// <summary>
        /// Вставка дуги Y после дуги X.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void InsertAfter(ref Arc x, ref Arc y)
        {
            if (!IsBlank(x.right))
            {
                x.right = y;
                y.parent = x;
            }
            else
            {
                x.next.left = y;
                y.parent = x.next.left;
            }

            if (!IsBlank(x.next))
                x.next.prev = y;
            y.next = x.next;
            x.next = y;
            y.prev = x;

            // Балансируем дерево.
            FixInserting(ref y);
        }

        private void FixInserting(ref Arc curArc)
        {
            // if (x.parent == null)
            // {
            //     mRoot.color = Arc.Color.BLACK;
            //     return;
            // }
            //
            // if (x.parent.color == Arc.Color.BLACK)
            //     return;

            Arc  x = curArc;
            // Родитель.
            Arc p = x.parent;

            while (p.color == Arc.Color.RED)
            {
                if (p == p.parent.left)
                {
                    // Дядя справа.
                    Arc u = p.parent.right;

                    // Case 1 "Дядя красный"
                    if (u.color == Arc.Color.RED)
                    {
                        p.color = Arc.Color.BLACK;
                        u.color = Arc.Color.BLACK;
                        p.parent.color = Arc.Color.RED;
                        x = p.parent;
                    }
                    else
                    {
                        // Case 2 "Дядя черный, малый поворот"
                        if (x == p.right)
                        {
                            x = p;
                            LeftRotate(ref p);
                        }

                        // Case 3
                        p.color = Arc.Color.BLACK;
                        p.parent.color = Arc.Color.RED;
                        RightRotate(ref p.parent);
                    }
                }
                else
                {
                    // Дядя слева.
                    Arc u = p.parent.left;

                    // Case 1 "Дядя красный"
                    if (u.color == Arc.Color.RED)
                    {
                        p.color = Arc.Color.BLACK;
                        u.color = Arc.Color.BLACK;
                        p.parent.color = Arc.Color.RED;
                        x = p.parent;
                    }
                    else
                    {
                        // Case 2 "Дядя черный, малый поворот"
                        if (x == p.left)
                        {
                            x = p;
                            RightRotate(ref p);
                        }

                        // Case 3
                        p.color = Arc.Color.BLACK;
                        p.parent.color = Arc.Color.RED;
                        LeftRotate(ref p.parent);
                    }
                }
            }

            // В любой непонятной ситуации красим корень в черный.
            mRoot.color = Arc.Color.BLACK;
        }

        private void LeftRotate(ref Arc x)
        {
            Arc y = x.right;
            x.right = y.left;
            if (!IsBlank(y.left))
                y.left.parent = x;
            y.parent = x.parent;
            if (!IsBlank(x.parent))
                mRoot = y;
            else if (x == x.parent.left)
                x.parent.left = y;
            else
                x.parent.right = y;
            y.left = x;
            x.parent = y;
        }

        private void RightRotate(ref Arc x)
        {
            Arc y = x.left;
            x.left = y.right;
            if (!IsBlank(y.right))
                y.right.parent = x;
            y.parent = x.parent;
            if (!IsBlank(x.parent))
                mRoot = y;
            else if (x == x.parent.left)
                x.parent.left = y;
            else
                x.parent.right = y;
            y.right = x;
            x.parent = y;
        }

        // private void GreatLeftRotate(ref Arc p)
        // {
        //     Arc g = p.parent;
        //     g.left = p.right;
        //     if (!IsBlank(g.left != null)
        //         g.left.parent = g;
        //     p.parent = g.parent;
        //     if (p.parent == null)
        //         mRoot = p;
        //     else if (g == p.parent.left)
        //         p.parent.left = p;
        //     else
        //         p.parent.right = p;
        //     p.right = g;
        //     g.parent = p;
        // }
        //
        // private void GreatRightRotate(ref Arc p)
        // {
        //     Arc g = p.parent;
        //     g.right = p.left;
        //     if (g.right != null)
        //         g.right.parent = g;
        //     p.parent = g.parent;
        //     if (p.parent == null)
        //         mRoot = p;
        //     else if (g == p.parent.left)
        //         p.parent.left = p;
        //     else
        //         p.parent.right = p;
        //     p.left = g;
        //     g.right = p;
        // }

        private double CalculateBreakPoint(Point leftPoint, Point rightPoint, double line)
        {
            double leftX = leftPoint.x, leftY = leftPoint.y, rightX = rightPoint.x, rightY = rightPoint.y;
            double leftDist = 1 / (2 * leftY - line);
            double rightDist = 1 / (2 * rightY - line);
            double a = leftDist - rightDist;
            double b = 2 * (rightX * rightDist - leftDist * leftDist);
            double c = (leftY * leftY + leftX * leftX - line * line) * leftDist -
                       (rightY * rightY + rightX * rightX - line * line) * rightDist;
            double delta = b * b - 4 * a * c;
            return (-b + Math.Sqrt(delta)) / (2 * a);
        }
    }
}