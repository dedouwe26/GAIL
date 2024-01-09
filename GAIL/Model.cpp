#include "GAIL.hpp"

#include <iostream>
#include <algorithm>
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
    bool Vertex::operator==(const Vertex& other) {
        return this->position == other.position && this->attributes == other.attributes;
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
    Mesh Mesh::FromObj(string path) {
        std::vector<Vertex*> vertices = std::vector<Vertex*>();
        std::vector<unsigned int> faces = std::vector<unsigned int>();

        std::ifstream file(path, std::ios::in);
        if (!file.is_open()) {
            std::cerr << "GAIL: FromOBJ (load file): Could not open \"" << path << "\"" << std::endl;
            return Mesh(vertices, faces);
        }

        std::vector<Vector2> UVs = std::vector<Vector2>();
        std::vector<Vector3> normals = std::vector<Vector3>();

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
                vertices.push_back(&Vertex(x, y, z));
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
                    unsigned v1 = stoi(i1.substr(0, pos));
                    unsigned n1 = stoi(i1.erase(0, pos + 2));
                    vertices[v1-1]->attributes["normal"] = NormalAttribute{normals[n1-1]};
                    
                    pos = i2.find("//");
                    unsigned v2 = stoi(i2.substr(0, pos));
                    unsigned n2 = stoi(i2.erase(0, pos + 2));
                    vertices[v2-1]->attributes["normal"] = NormalAttribute{normals[n2-1]};

                    pos = i3.find("//");
                    unsigned v3 = stoi(i3.substr(0, pos));
                    unsigned n3 = stoi(i3.erase(0, pos + 2));
                    vertices[v3-1]->attributes["normal"] = NormalAttribute{normals[n3-1]};
                } else if (i1.find('/') != std::string::npos) {
                    size_t pos = i1.find("/");
                    if (i1.find("/", pos+1)==std::string::npos) {
                        unsigned v1 = stoi(i1.substr(0, pos));
                        unsigned t1 = stoi(i1.erase(0, pos + 1));
                        vertices[v1-1]->attributes["uv"] = UVAttribute{UVs[t1-1]};

                        pos = i2.find("/");
                        unsigned v2 = stoi(i2.substr(0, pos));
                        unsigned t2 = stoi(i2.erase(0, pos + 1));
                        vertices[v2-1]->attributes["uv"] = UVAttribute{UVs[t2-1]};

                        pos = i3.find("/");
                        unsigned v3 = stoi(i3.substr(0, pos));
                        unsigned t3 = stoi(i3.erase(0, pos + 1));
                        vertices[v3-1]->attributes["uv"] = UVAttribute{UVs[t3-1]};
                        faces.push_back(v1-1);
                        faces.push_back(v2-1);
                        faces.push_back(v3-1);
                    } else {
                        unsigned v1 = stoi(i1.substr(0, pos));
                        unsigned t1 = stoi(i1.erase(0, pos + 1));
                        pos = i1.find("/");
                        unsigned n1 = stoi(i1.erase(0, pos + 1));
                        vertices[v1-1]->attributes["normal"] = NormalAttribute{normals[n1-1]};
                        vertices[v1-1]->attributes["uv"] = UVAttribute{UVs[t1-1]};

                        pos = i2.find("/");
                        unsigned v2 = stoi(i2.substr(0, pos));
                        unsigned t2 = stoi(i2.erase(0, pos + 1));
                        pos = i2.find("/");
                        unsigned n2 = stoi(i2.erase(0, pos + 1));
                        vertices[v2-1]->attributes["normal"] = NormalAttribute{normals[n2-1]};
                        vertices[v2-1]->attributes["uv"] = UVAttribute{UVs[t2-1]};

                        pos = i3.find("/");
                        unsigned v3 = stoi(i3.substr(0, pos));
                        unsigned t3 = stoi(i3.erase(0, pos + 1));
                        pos = i3.find("/");
                        unsigned n3 = stoi(i3.erase(0, pos + 1));
                        vertices[v3-1]->attributes["normal"] = NormalAttribute{normals[n3-1]};
                        vertices[v3-1]->attributes["uv"] = UVAttribute{UVs[t3-1]};

                        faces.push_back(v1-1);
                        faces.push_back(v2-1);
                        faces.push_back(v3-1);
                    }
                } else {
                    faces.push_back(stoi(i1));
                    faces.push_back(stoi(i2));
                    faces.push_back(stoi(i3));
                }
            }  
        }

        file.close();

        return Mesh(vertices, faces);
    };
    Mesh::Mesh(std::vector<Face> faces) {
        for(Face face : faces) {
            std::vector<GAIL::Vertex *>::iterator iter = std::find(vertices.begin(), vertices.end(), face.p1);
            if(iter != vertices.end()) {
                this->indexFaces.push_back(iter - vertices.begin());
            } else {
                this->vertices.push_back(face.p1);
                this->indexFaces.push_back(vertices.size()-1);
            };
            iter = std::find(vertices.begin(), vertices.end(), face.p2);
            if(iter != vertices.end()) {
                this->indexFaces.push_back(iter - vertices.begin());
            } else {
                this->vertices.push_back(face.p2);
                this->indexFaces.push_back(vertices.size()-1);
            };
            iter = std::find(vertices.begin(), vertices.end(), face.p3);
            if(iter != vertices.end()) {
                this->indexFaces.push_back(iter - vertices.begin());
            } else {
                this->vertices.push_back(face.p3);
                this->indexFaces.push_back(vertices.size()-1);
            };

        };
    }
    Mesh::~Mesh() {indexFaces.clear(); vertices.clear();};
    Mesh::Mesh(std::vector<Vertex*> vertices, std::vector<unsigned> indexFaces) : indexFaces{indexFaces}, vertices{vertices} {};
    Model::Model(Mesh mesh, BaseMaterial material) : mesh{mesh}, material{material} {};
    Model::~Model() {
        delete &mesh;
        delete &material;
    };
    InstancedModel::InstancedModel(Mesh* mesh, BaseMaterial material) : mesh{mesh}, material{material} {};
    InstancedModel::~InstancedModel() {
        delete mesh;
        delete &material;
    };
};