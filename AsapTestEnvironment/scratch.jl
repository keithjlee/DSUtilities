using Asap, AsapToolkit, JSON

#meta
begin
    nx = 10
    dx = 1500.
    ny = 14
    dy = 1250.
    dz = 2000.

    section = toASAPtruss(rand(allLL()), Steel_Nmm.E)

    load = [0., 0., -10e3]
    support = :x
end

#generate
sf = SpaceFrame(nx, dx, ny, dy, dz, section; support = support, load = load)
model = sf.model

node_positions = getproperty.(model.nodes, :position)
node_dof = getproperty.(model.nodes, :dof)
node_ids = string.(getproperty.(model.nodes, :id))

element_connectivity = [element.nodeIDs .- 1 for element in model.elements]

rawdata = JSON.json(model)

open("test.json", "w") do f
    write(f, rawdata)
end