#include "controller.h"

void Controller::new_target(int floor)
{
    qDebug() << "Get new target: floor №" << floor + 1;
    is_target[floor] = true;

    if (state == FREE)
        next_target();
}

void Controller::next_target()
{
    if (is_target[cur_floor])
    {
        is_target[cur_floor] = false;
        emit panel_new_target(cur_floor, STAY);
    }
    else
    {
        for (int i = 0; i < is_target.size(); i++)
            if (is_target[i])
            {
                is_target[i] = false;
                if (i > cur_floor)
                    emit panel_new_target(i, UP);
                else
                    emit panel_new_target(i, DOWN);
                break;
            }
    }
}

void Controller::busy(int floor, const direction &_direction)
{
    if (state == FREE)
    {
        state = BUSY;
        cur_target = floor;
        cur_dir = _direction;
    }
    else if (state == BUSY)
    {
        qDebug() << "Passed floor №" << floor + 1;
        cur_floor += cur_dir;
    }
}

void Controller::free(int floor)
{
    state = FREE;
    cur_floor = floor;
    next_target();
}
