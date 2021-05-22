using PriorityQueue;
using System;
namespace VoronoiDiagramLib
{
    public class Fortune
    {
        // Очередь с приоритетом событий по Site.basePoint.y.
        private PriorityQueue<Event> mEvents = new PriorityQueue<Event>();

        // Береговая линия.
        private RedBlackLine mBeachLine;

        // Сама диаграмма Вороного.
        private VoronoiDiagram mDiagram = new VoronoiDiagram();

        // Заметающая прямая.
        private double line;

        private Random rnd = new Random();

        /// <summary>
        /// Конструктор алгоритма Форчуна.
        /// </summary>
        /// <param name="nEvents"></param>
        /// <param name="xLim"></param>
        /// <param name="yLim"></param>
        public Fortune(int nEvents, int xLim, int yLim)
        {
            // Заполняем очередь событий cгенерированными событиями.
            EventGeneration(nEvents, xLim, yLim);

            // Cоздаем береговую линию.
            mBeachLine = new RedBlackLine();

            // Обрабатываем по очереди все собития из очереди событий.
            while (!mEvents.Empty)
            {
                Event curEvent = mEvents.Pop().Value;
                line = curEvent.eventPoint.y;
                if (curEvent is SiteEvent)
                    SiteEventHandler((SiteEvent)curEvent);
                else
                    CircleEventHandler((CircleEvent) curEvent);
            }
        }

        /// <summary>
        /// Обработчик события точки.
        /// </summary>
        /// <param name="curEvent"></param>
        private void SiteEventHandler(SiteEvent curEvent)
        {
            Site curSite = curEvent.site;

            // 1) Если береговая линия пуста то просто ставим новую дугу в корень.
            if (mBeachLine.IsEmpty())
            {
                mBeachLine.SetRoot(new Arc(curSite));
                return;
            }

            // 2) Ищем дугу которую разбивает наше новое собите точки.
            Arc brokenArc = mBeachLine.FindArcAbove(curSite.basePoint, line);

            // 3) Удаляем событие которе было привязано к BrokenArc.
            DeleteEvent(ref brokenArc);

            // 4) Разбиваем найденную дугу и вставляем между разбитыми половинками нашу новую дугу.
            Arc midleArc = BreakArc(ref brokenArc, curSite);
            Arc leftArc = midleArc.prev;
            Arc rightArc = midleArc.next;

            // 5) Добавляем рёбра между новыми дугами.
            AddEdge(ref leftArc, ref midleArc);
            AddEdge(ref midleArc, ref rightArc);
            midleArc.rightHalfEdge = midleArc.leftHalfEdge;
            leftArc.rightHalfEdge = rightArc.leftHalfEdge;

            // 6) Проверям существование события окружности.
            // Для новой левой тройки.
            if (!mBeachLine.IsBlank(leftArc.prev))
                NewCircleEvent(ref leftArc.prev, ref leftArc, ref midleArc);
            // Для новой правой тройки.
            if (!mBeachLine.IsBlank(rightArc.next))
                NewCircleEvent(ref midleArc, ref rightArc, ref rightArc.next);
        }

        /// <summary>
        /// Обработчик события окружности.
        /// </summary>
        /// <param name="curCircleEvent"></param>
        private void CircleEventHandler(CircleEvent curCircleEvent)
        {
            Arc baseArc = curCircleEvent.baseArc;
            // Добавляем новую вершину диграмы Вороного.
            Vertex newVertex = mDiagram.CreateVertex(curCircleEvent.circlePoint);
            DeleteEvent(ref baseArc.prev);
            DeleteEvent(ref baseArc.next);
            // РЕАЛИЗОВАТЬ.
            DeleteArc(ref baseArc, ref newVertex);
            // 6) Проверям существование события окружности.
            // Для новой левой тройки.
            if (!mBeachLine.IsBlank(baseArc.prev.prev))
                NewCircleEvent(ref baseArc.prev.prev, ref baseArc.prev, ref baseArc.next);
            // Для новой правой тройки.
            if (!mBeachLine.IsBlank(baseArc.next.next))
                NewCircleEvent(ref baseArc.prev, ref baseArc.next, ref baseArc.next.next);
        }

        /// <summary>
        /// Разбиение дуги новой дугой на 3.
        /// </summary>
        /// <param name="brokenArc"></param>
        /// <param name="curSite"></param>
        /// <returns></returns>
        private Arc BreakArc(ref Arc brokenArc, Site curSite)
        {
            // Создаем новые дуги.
            Arc midleArc = new Arc(curSite);
            Arc leftArc = new Arc(brokenArc.site);
            Arc rightArc = new Arc(brokenArc.site);

            // Запоминаем крайние ребра разбитой дуги.
            leftArc.leftHalfEdge = brokenArc.leftHalfEdge;
            rightArc.rightHalfEdge = brokenArc.rightHalfEdge;

            // Меняем дугу brokenArc в береговой линии на дугу middleArc а потом привязываем к ней остатки дуги brokenArc слева и справа.
            mBeachLine.Replace(ref brokenArc, ref midleArc);
            mBeachLine.InsertBefore(ref midleArc, ref leftArc);
            mBeachLine.InsertAfter(ref midleArc, ref rightArc);
            return midleArc;
        }

        private void NewCircleEvent(ref Arc leftArc, ref Arc midleArc, ref Arc rightArc)
        {
            Point eventPoint = new Point();
            Point centerCircle = CalculateCicrcle(leftArc.site.basePoint, midleArc.site.basePoint, rightArc.site.basePoint, ref eventPoint);
            bool leftBelow = leftArc.site.basePoint.y < midleArc.site.basePoint.y;
            bool rightBelow = rightArc.site.basePoint.y < midleArc.site.basePoint.y;
            if (leftBelow && rightBelow && eventPoint.y <= line)
            {
                Node<Event> newNode = new Node<Event>(new CircleEvent(eventPoint, centerCircle, ref midleArc));
                mEvents.Add(newNode);
                midleArc.circleEvent = newNode;
            }
        }

        public Point CalculateCicrcle(Point fPoint, Point sPoint, Point tPoint, ref Point eventPoint)
        {
            double x12 = fPoint.x - sPoint.x;
            double x23 = sPoint.x - tPoint.x;
            double x31 = tPoint.x - fPoint.x;
            double y12 = fPoint.y - sPoint.y;
            double y23 = sPoint.y - tPoint.y;
            double y31 = tPoint.y - fPoint.y;
            double z1 = fPoint.x * fPoint.x + fPoint.y * fPoint.y;
            double z2 = sPoint.x * sPoint.x + sPoint.y * sPoint.y;
            double z3 = tPoint.x * tPoint.x + tPoint.y * tPoint.y;
            double zx = y12 * z3 + y23 * z1 + y31 * z2;
            double zy = x12 * z3 + x23 * z1 + x31 * z2;
            double z = x12 * y31 - y12 *  x31;
            double a = -1 * zx / (2 * z);
            double b = zy / (2 * z);
            double r = Math.Sqrt(Math.Pow(fPoint.x - a, 2) + Math.Pow(fPoint.y - b, 2));
            eventPoint = new Point(a, b + r);
            return new Point(a, b);
        }


        /// <summary>
        /// Добавляет ребра между двумя дугами, которые они рисуют в процессе алгоритма.
        /// </summary>
        /// <param name="leftArc"></param>
        /// <param name="rightArc"></param>
        private void AddEdge(ref Arc leftArc, ref Arc rightArc)
        {
            leftArc.rightHalfEdge = mDiagram.CreateHalfEdge(leftArc.site);
            rightArc.leftHalfEdge = mDiagram.CreateHalfEdge(rightArc.site);

            // Привязываем близнецов.
            leftArc.rightHalfEdge.twin = rightArc.leftHalfEdge;
            rightArc.leftHalfEdge.twin = leftArc.rightHalfEdge;
            // ПЕРЕПИСАТЬ СПИСОК РЕБЕР НА СВОЙ И ДОБАВИТЬ БЛИЗНЕЦОВ ТУДА.
            // РЕШЕНО, НО НУЖНО ПРОВЕРИТЬ.
        }

        /// <summary>
        /// Удаляет событие окружности связанное с дуго curArc.
        /// </summary>
        /// <param name="curArc"></param>
        private void DeleteEvent(ref Arc curArc)
        {
            mEvents.Delete(curArc.circleEvent);
            curArc.circleEvent = null;
        }

        private void  DeleteArc(ref Arc curArc, ref Vertex newVert)
        {
            // Привязываем вершину к ребрам Вороного.
            curArc.left.rightHalfEdge.origin = newVert;
            curArc.right.leftHalfEdge.destionation = newVert;
            curArc.leftHalfEdge.destionation = newVert;
            curArc.rightHalfEdge.origin = newVert;

            // Привязываем ребра Вороного между собой.
            curArc.leftHalfEdge.next = curArc.rightHalfEdge;
            curArc.rightHalfEdge.prev = curArc.leftHalfEdge;

            // Удаляем дугу из береговой линии.
            mBeachLine.DeleteArc(ref curArc);

            // Старые ребра.
            HalfEdge prevRight = curArc.prev.rightHalfEdge;
            HalfEdge nextLeft = curArc.next.leftHalfEdge;

            // Новые ребра Вороного.
            AddEdge(ref curArc.prev, ref curArc.next);

            // Для новых ребер Вороного.
            curArc.prev.rightHalfEdge.destionation = newVert;
            curArc.next.leftHalfEdge.origin = newVert;

            // Связываем старые и новые ребра для левой дуги.
            curArc.prev.rightHalfEdge.next = prevRight;
            prevRight.prev = curArc.prev.rightHalfEdge;

            // Связываем старые и новые ребра для правой дуги.
            nextLeft.next = curArc.next.leftHalfEdge;
            curArc.next.leftHalfEdge.prev = nextLeft;
        }

        /// <summary>
        /// Первоначальная генерация Site и добавление в очередь с приоритетами событий SiteEvent-ы.
        /// </summary>
        /// <param name="nEvents"></param>
        /// <param name="xLim"></param>
        /// <param name="yLim"></param>
        private void EventGeneration(int nEvents, int xLim, int yLim)
        {
            for (int i = 0; i < nEvents; i++)
            {
                SiteEvent curEvent = new SiteEvent(new Point(rnd.Next(1, xLim), rnd.Next(1, yLim)));
                mEvents.Add(curEvent);
            }
        }
    }
}