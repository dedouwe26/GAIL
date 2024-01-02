#include "GAIL.hpp"

#include <iostream>
#include <fstream>

namespace GAIL {
    Vertex::Vertex() {this->position={0,0,0};};
    Vertex::Vertex(double x, double y, double z) {this->position={x,y,z};};
    Vertex::Vertex(double x, double y) {this->position = {x,y,0};};
    Vertex::~Vertex() {
        attributes.clear();
        delete &attributes;
        delete &position;
    };
    Face::Face(Vertex *indices[3]) {
        this->p1 = indices[0];
        this->p2 = indices[1];
        this->p3 = indices[2];
    };

    Face::Face(Vertex p1, Vertex p2, Vertex p3) {
        this->p1 = &p1;
        this->p2 = &p2;
        this->p3 = &p3;
    };
    Face::~Face() {
        delete p1;
        delete p2;
        delete p3;
    };
    Mesh Mesh::FromObj(string path)
    {
        std::vector<Vertex> vertices = std::vector<Vertex>();
        std::vector<unsigned int> faces = std::vector<unsigned int>();

        // Load obj file
        std::ifstream file(path, std::ios::in);
        if (!file.is_open()) {
            std::cerr << "GAIL: FromOBJ (load file): Could not open \"" << path << "\"" << std::endl;
            return Mesh(vertices, faces);
        }

        std::vector<Vector2> UVs = std::vector<Vector2>();
        std::vector<Vector3> normals = std::vector<Vector3>();

        // Read file line by line
        std::string line;
        while (std::getline(file, line)) {
            if (line[0] == '#') continue;

            std::stringstream ss = std::stringstream(line);
            std::string keyword;
            std::string data;
            ss >> keyword;
            if (keyword == "v") {
                double x, y, z;
                ss >> x >> y >> z;
                vertices.push_back(Vertex(x, y, z));
            } else if (keyword == "vt") {
                double x, y;
                ss >> x >> y;
                UVs.push_back(Vector2{x, y});
            } else if (keyword == "vn") {
                double x, y, z;
                ss >> x >> y >> z;
                normals.push_back(Vector3{x, y, z});
            } else if (keyword == "f") {
                std::string i1, i2, i3;
                ss >> i1 >> i2 >> i3;
                if (i1.find('//') != std::string::npos) {
                    size_t pos = i1.find("//");
                    std::string v1 = i1.substr(0, pos);
                    std::string n1 = i1.erase(0, pos + 2);
                    
                    pos = i2.find("//");
                    std::string v2 = i2.substr(0, pos);
                    std::string n2 = i2.erase(0, pos + 2);

                    pos = i3.find("//");
                    std::string v3 = i3.substr(0, pos);
                    std::string n3 = i3.erase(0, pos + 2);
                } else if (i1.find('/') != std::string::npos) {
                    size_t pos = i1.find("/");
                    if (i1.find("/", pos+1)==std::string::npos) {
                        std::string v1 = i1.substr(0, pos);
                        std::string t1 = i1.erase(0, pos + 1);
                        pos = i2.find("/");
                        std::string v2 = i2.substr(0, pos);
                        std::string t2 = i2.erase(0, pos + 1);
                        pos = i3.find("/");
                        std::string v3 = i3.substr(0, pos);
                        std::string t3 = i3.erase(0, pos + 1);
                    } else {
                        std::string v1 = i1.substr(0, pos);
                        std::string t1 = i1.erase(0, pos + 1);
                        pos = i1.find("/");
                        std::string n1 = i1.erase(0, pos + 1);

                        pos = i2.find("/");
                        std::string v2 = i2.substr(0, pos);
                        std::string t2 = i2.erase(0, pos + 1);
                        pos = i2.find("/");
                        std::string n2 = i2.erase(0, pos + 1);

                        pos = i3.find("/");
                        std::string v3 = i3.substr(0, pos);
                        std::string t3 = i3.erase(0, pos + 1);
                        pos = i3.find("/");
                        std::string n3 = i3.erase(0, pos + 1);
                    }
                } else {
                    std::string v1 = i1;
                    std::string v2 = i2;
                    std::string v3 = i3;
                }
            };
            
        }

        file.close();

        return Mesh(vertices, faces);
    };

};