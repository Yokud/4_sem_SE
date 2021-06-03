#ifndef LIFT_H
#define LIFT_H

#include <QObject>
#include "controller.h"
#include "cabin.h"

class Lift : public QObject
{
    Q_OBJECT
public:
    Lift();
    void call(int floor);

private:
    Cabin cabin;
    Controller controller;
};

#endif // LIFT_H
