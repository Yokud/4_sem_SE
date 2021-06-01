#ifndef CAMERA_H
#define CAMERA_H

#include "object.h"


    class Camera: public InvisibleObject
    {
    public:
        Camera() = default;
        Camera(const Point &pos) : current_pos(pos) {};
        ~Camera() = default;

        virtual ObjIterator begin() override { return ObjIterator();};
        virtual ObjIterator end() override { return ObjIterator();};

        virtual void reform(const Point &pos, const Point &turn, const Point &scale) override;
        virtual void accept(std::shared_ptr<Visitor> visitor) override;
        Point getPos() { return current_pos; };

        virtual std::string info() override { return "It's camera";};
    private:
        Point current_pos;
    };



#endif // CAMERA_H
