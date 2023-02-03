/*
--Examples--
run("../clearAll.js"); //clear all model and another objects in RFEM
include("../Functions/global.js"); //import this script
throw new Error("text"); //error message
*/


function isObjectEmpty(object)
{
    /*
     Returns boolean if object exist.
    */
    for(var prop in object) {
        if(object.hasOwnProperty(prop))
            return false;
    }

    return true;
}


//---------------------- Script Functions ----------------------
function createCoordinateSystem(index, vector, options)
{
    /*
    Create Coordinate System
    Example:
        createCoordinateSystem(int, type, vector);
        createCoordinateSystem(1, $V([0, 0, 0.04]), {type: coordinate_systems.TYPE_2_POINTS_AND_ANGLE, comment:"Nazev", u_axis_point_coordinates: $V([0.1, 1.2, -2.3]),uw_plane_angle:1)};
    */
    var coordinate_system = coordinate_systems.create(index);
    coordinate_system.type = coordinate_systems.TYPE_3_POINTS;
    coordinate_system.origin_coordinates = vector;
    if (isObjectEmpty(options))
    {
        //Default settings
    }
    else
    {
        //User settings options
        Object.keys(options).map(function(key){return coordinate_systems[index][key] = options[key]});
    }

    return coordinate_system;
}

function createNode(index, coordinates, options)
{
    /*
    Create node on specific coordinates
    Example:
        createNode(int, vector));
        createNode(1, $V([0, 0, 0.04]));
        createNode(1, $V([0, 0, 0.04]), {support:1});
    */
    var node = nodes.create(index);
    node.coordinates = coordinates;
    if (isObjectEmpty(options))
    {
        //Default settings
    }
    else
    {
        //User settings options
        Object.keys(options).map(function(key){return nodes[index][key] = options[key]});
    }

    return node;
}

function createLine(index, nodes, options)
{
    /*
    Create line connecting definition nodes
    Example:
        createLine(int, str);
        createLine( 1, '1,2');
        createLine( 1, '1,2',{type: lines.TYPE_ARC, arc_control_point: $V([1, 2, 3])});
    */
    if (RSTAB)
    {
        return;
    }
    var line = lines.create(index);
    line.definition_nodes = nodes;
    if (isObjectEmpty(options))
    {
        //Default settings
        lines[index].type = lines.TYPE_POLYLINE;
    }
    else
    {
        //User settings options
        Object.keys(options).map(function(key){return lines[index][key] = options[key]});
    }

    return line;
}

function createLineSet(index, lines, options)
{
    /*
    Create line set
    Example:
        createLineSet(int, str);
        createLineSet(1, '1,2');
        createLineSet(1, '1,2', {set_type: line_sets.SET_TYPE_GROUP});
    */
    if (RSTAB)
    {
        return;
    }
    var line_set = line_sets.create(index);
    line_set.lines = lines;
    if (isObjectEmpty(options))
    {
        //Default settings
        line_sets[index].set_type = line_sets.SET_TYPE_CONTINUOUS;
    }
    else
    {
        //User settings options
        Object.keys(options).map(function(key){return line_sets[index][key] = options[key]});
    }

    return line_set;
}

function createMember(index, nodes, options)
{
    /*
    Create line with member connecting definition nodes
    Example:
        createMember(int, str);
        createMember( 1, '1,2');
        createMember(6, '1,5',{section_start: 2, member_hinge_start: 1});
    */
    var member =  members.create(index);
    if (nodes!="") {
        if (RFEM)
        {
            var line = lines.create(lines.lastId()+1);
            line.definition_nodes = nodes;
            member.line = line.no;
        }
        else
        {
            var definition_nodes = nodes.toString().split(',');
            member.node_start = definition_nodes[0];
            member.node_end = definition_nodes[1];
        }
    }
    if (isObjectEmpty(options))
    {
        //Default settings
        member.type = members.TYPE_BEAM;
        member.section_start = 1;
    }
    else
    {
        //User settings options
        Object.keys(options).map(function(key){return member[key] = options[key]});
    }

    return {
        line: line,
        member: member
    }
}

function createMember2(index, lines, options)
{
    /*
    Create member on defined line connecting
    Example:
        createMember(int, str);
        createMember( 1, '1,2');
        createMember(6, '1,5',{section_start: 2, member_hinge_start: 1});
    */
    var member =  members.create(index);
    members[index].line = lines;
    if (isObjectEmpty(options))
    {
        //Default settings
        members[index].type = members.TYPE_BEAM;
        members[index].section_start = 1;
    }
    else
    {
        //User settings options
        Object.keys(options).map(function(key){return members[index][key] = options[key]});
    }

    return {
        member: member
    }
}

function createMemberSet(index, members, options)
{
    /*
    Create member set
    Example:
        createMemberSet(int, str);
        createMemberSet(1, '1,2');
        createMemberSet(1, '1,2', {set_type: member_sets.SET_TYPE_GROUP});
    */
    var member_set = member_sets.create(index);
    member_set.members = members;
    if (isObjectEmpty(options))
    {
        //Default settings
        member_sets[index].set_type = member_sets.SET_TYPE_CONTINUOUS;
    }
    else
    {
        //User settings options
        Object.keys(options).map(function(key){return member_sets[index][key] = options[key]});
    }

    return member_set;
}

function createMemberHinge(index, springs)
{
    /*
    Create member hinge
    Example:
        createMemberHinge(int, vector);
        createMemberHinge( 1, [1, 2, 3, 4, 5, 6]);
        createMemberHinge( 2, ["inf", "inf", 0, 0, "inf", 0]);
    */
    var member_hinge = member_hinges.create(index);
    member_hinge.axial_release_n = springs[0];
    member_hinge.axial_release_vy = springs[1];
    member_hinge.axial_release_vz = springs[2];
    member_hinge.moment_release_mt = springs[3];
    member_hinge.moment_release_my = springs[4];
    member_hinge.moment_release_mz = springs[5];

    return member_hinge;
}

function createSurface(index, lines, options)
{
    /*
    Create surface connecting boundary lines
    Example:
        createSurface(int, str, object);
        createSurface(1, '4,10,9,11');
        createSurface(6, '35,41,28,27,34',{type: surfaces.TYPE_STANDARD, thickness: 5});
    */
    if (RSTAB)
    {
        return;
    }
    var surface = surfaces.create(index);
    surface.boundary_lines = lines;
    if (isObjectEmpty(options))
    {
        //Default settings
        surfaces[index].type = surfaces.TYPE_WITHOUT_THICKNESS;
    }
    else
    {
        //User settings options
        Object.keys(options).map(function(key){return surfaces[index][key] = options[key]});
    }

    return surface;
}

function createSurfaceSet(index, surfaces, options)
{
    /*
    Create surface set
    Example:
        createSurfaceSet(int, str);
        createSurfaceSet(1, '1,2');
        createSurfaceSet(1, '1,2', {set_type: surface_sets.SET_TYPE_GROUP});
    */
    if (RSTAB)
    {
        return;
    }
    var surface_set = surface_sets.create(index);
    surface_set.surfaces = surfaces;
    if (isObjectEmpty(options))
    {
        //Default settings
        surface_sets[index].set_type = surface_sets.SET_TYPE_CONTINUOUS;
    }
    else
    {
        //User settings options
        Object.keys(options).map(function(key){return surface_sets[index][key] = options[key]});
    }

    return surface_set;
}

function createSolid(index, surfaces, options)
{
    /*
    Create surface connecting boundary lines
    Example:
        createSolid(int, str);
        createSolid(1, '6-12');
        createSolid(1, '6-12',{material: 1,type: solids.TYPE_GAS, gas: 1});
    */
    var solid = solids.create(index)
    solid.boundary_surfaces = surfaces;
    if (isObjectEmpty(options))
    {
        //Default settings
        solids[index].material = 1;
    }
    else
    {
        //User settings options
        Object.keys(options).map(function(key){return solids[index][key] = options[key]});
    }

    return solid;
}

function createSolidSet(index, solids, options)
{
    /*
    Create solid set
    Example:
        createSolidSet(int, str);
        createSolidSet(1, '1,2');
        createSolidSet(1, '1,2', {set_type: solid_sets.SET_TYPE_GROUP});
    */
    if (RSTAB)
    {
        return;
    }
    var solid_set = solid_sets.create(index);
    solid_set.solids = solids;
    if (isObjectEmpty(options))
    {
        //Default settings
        solid_sets[index].set_type = solid_sets.SET_TYPE_CONTINUOUS;
    }
    else
    {
        //User settings options
        Object.keys(options).map(function(key){return solid_sets[index][key] = options[key]});
    }

    return solid_set;
}

function createSolidGas(index, pressure, temperature)
{
    /*
    Create solid gas
    Example:
        createSolidGas(int, float, float);
        createSolidGas(1, 1200, 120);
    */
    var solid_gas = solid_gases.create(index);
    solid_gas.pressure = pressure;
    solid_gas.temperature = temperature;

    return solid_gas;
}

function createOpening(index, lines)
{
    /*
    Create opening connecting boundary lines on surface
    Example:
        createOpening(int, str);
        createOpening(1, '6-12');
    */
    if (RSTAB)
    {
        return;
    }
    var opening = openings.create(index);
    opening.boundary_lines = lines;

    return opening;
}

function createNodalSupport(index, nodes, springs)
{
    /*
    Create nodal support
    Example:
        createNodalSupport(int, str, vector);
        createNodalSupport( 1, '1,2', [1, 2, 3, 4, 5, 6]);
        createNodalSupport( 2, '1,2', ["inf", "inf", 0, 0, "inf", 0]);
    */
    var node_support = nodal_supports.create(index);
    node_support.nodes = nodes;
    node_support.spring_x = springs[0];
    node_support.spring_y = springs[1];
    node_support.spring_z = springs[2];
    node_support.rotational_restraint_x = springs[3];
    node_support.rotational_restraint_y = springs[4];
    node_support.rotational_restraint_z = springs[5];

    return node_support;
}

function createLineSupport(index, lines, springs)
{
    /*
    Create line support
    Example:
        createLineSupport(int, str, vector);
        createLineSupport( 1, '1,2', [1, 2, 3, 4, 5, 6]);
        createLineSupport( 2, '1,2', ["inf", "inf", 0, 0, "inf", 0]);
    */
    if (RSTAB)
    {
        return;
    }
    var line_support = line_supports.create(index);
    line_support.lines = lines;
    line_support.spring_x = springs[0];
    line_support.spring_y = springs[1];
    line_support.spring_z = springs[2];
    line_support.rotational_restraint_x = springs[3];
    line_support.rotational_restraint_y = springs[4];
    line_support.rotational_restraint_z = springs[5];

    return line_support;
}

function createMemberSupport(index, members, springs)
{
    /*
    Create member support
    Example:
        createMemberSupport(int, str, vector);
        createMemberSupport( 1, '1,2', [1, 2, 3, 4, 5, 6, 7]);
        createMemberSupport( 2, '1,2', ["inf", "inf", 0, 0, "inf", 0, "inf"]);
    */
    var member_support = member_supports.create(index);
    member_support.members = members;
    member_support.spring_translation_x = springs[0];
    member_support.spring_translation_y = springs[1];
    member_support.spring_translation_z = springs[2];
    member_support.spring_shear_x = springs[3];
    member_support.spring_shear_y = springs[4];
    member_support.spring_shear_z = springs[5];
	member_support.spring_rotation = springs[6];

    return member_support;
}

function createSurfaceSupport(index, surfaces, springs)
{
    /*
    Create surface support
    Example:
        createSurfaceSupport(int, str, vector);
        createSurfaceSupport( 1, '1,2', [1, 2, 3, 4, 5]);
        createSurfaceSupport( 2, '1,2', ["inf", "inf", 0, "inf", "inf"]);
    */
    if (RSTAB)
    {
        return;
    }
    var surface_support = surface_supports.create(index);
    surface_support.surfaces = surfaces;
    surface_support.shear_xz = springs[3];
    surface_support.shear_yz = springs[4];
    surface_support.translation_x = springs[0];
    surface_support.translation_y = springs[1];
    surface_support.translation_z = springs[2];
    return surface_support;
}

function createNodalLoad(index, load_case, nodes, force_vector)
{
    /*
    Create nodal load in defined load case on define nodes (loading by 3 - vector of forces)
    Example:
        createNodalLoad(int, int, str, vector);
        createNodalLoad( 1, 4, '1,2', $V([1, 2, 3]));
    */
    var nodal_load = load_cases[load_case].nodal_loads.create(index);
    nodal_load.load_type = nodal_loads.LOAD_TYPE_COMPONENTS;
    nodal_load.components_force = force_vector;
    nodal_load.nodes = nodes;

    return nodal_load;
}

function createLineLoad(index, load_case, lines, magnitude)
{
    /*
    Create line load in defined load case on define lines
    Example:
        createLineLoad(int, int, str, float);
        createLineLoad( 1, 1, '1,2', 1.25);
    */
    if (RSTAB)
    {
        return;
    }
    var line_load = load_cases[load_case].line_loads.create(index);
    line_load.magnitude = magnitude;
    line_load.lines = lines;

    return line_load;
}

function createMemberLoad(index, load_case, members, magnitude)
{
    /*
    Create member load in defined load case on define members
    Example:
        createMemberLoad(int, int, str, float);
        createMemberLoad( 1, 1, '1,2', 1.25);
    */
    var member_load = load_cases[load_case].member_loads.create(index);
    member_load.magnitude = magnitude;
    member_load.members = members;

    return member_load;
}

function createMemberImperfection(index, load_case, imperfection_case, members, basic_value_absolute)
{
    /*
    Create member load in defined load case on define members
    Example:
        createMemberLoad(int, int, int, str, float);
        createMemberLoad( 1, 1, 1, '1,2', 1.25);
    */
    var member_imperfection = imperfection_cases[imperfection_case].member_imperfections.create(index);
    member_imperfection.basic_value_absolute=basic_value_absolute;
    member_imperfection.members = members;
    return member_imperfection;
}

function createSurfaceLoad(index, load_case, surfaces, uniform_magnitude)
{
    /*
    Create surface load in defined load case on define surfaces
    Example:
        createSurfaceLoad(int, int, str, float);
        createSurfaceLoad( 1, 4, '1,2', 1.25));
    */
    if (RSTAB)
    {
        return;
    }
    var surface_load = load_cases[load_case].surface_loads.create(index);
    surface_load.load_type = surface_loads.LOAD_TYPE_FORCE;
    surface_load.uniform_magnitude = uniform_magnitude;
    surface_load.surfaces = surfaces;

    return surface_load;
}

function createMemberEccentricity(index, vector, options)
{
    /*
    Create member load in defined load case on define members
    Example:
        createMemberEccentricity(int, vector);
        createMemberEccentricity(1, [0,0,0]);
        createMemberEccentricity(1, [0,0,0],{transverse_offset_vertical_alignment: ALIGN_TOP, transverse_offset_horizontal_alignment: ALIGN_MIDDLE});
    */

    var eccentricity = member_eccentricities.create(index);
    if (isObjectEmpty(options))
    {
        //Default settings
        member_eccentricities[index].specification_type = member_eccentricities.TYPE_ABSOLUTE;
        member_eccentricities[index].offset = vector;
    }
    else
    {
        //User settings options
        member_eccentricities[index].specification_type = member_eccentricities.TYPE_RELATIVE_AND_ABSOLUTE;
        Object.keys(options).map(function(key){return member_eccentricities[index][key] = options[key]});
        member_eccentricities[index].offset = vector;
    }

    return eccentricity;
}

function createSurfaceContact(index, group1, group2, contact_type)
{
    /*
    Create surface contact between two surfaces
    Example:
        createSurfaceLoad(int, str, str, int);
        createSurfaceContact(index, group1, group2, contact_type);
    */
    if (RSTAB)
    {
        return;
    }
    var surfaces_contact = surfaces_contacts.create(index);
    surfaces_contact.surfaces_group1 = group1;
    surfaces_contact.surfaces_group2 = group2;
    surfaces_contact.surfaces_contact_type = contact_type;

    return surfaces_contact;
}

//---------------------- User Functions ----------------------
function createStandardStaticAnalysisSettings()
{
    /*
    Create 3 standard Static Analysis Settings
    Example:
        createStandardStaticAnalysisSettings();
    */
    static_analysis_settings.create(1);
    static_analysis_settings[1].analysis_type = static_analysis_settings.GEOMETRICALLY_LINEAR;

    static_analysis_settings.create(2);
    static_analysis_settings[2].analysis_type = static_analysis_settings.SECOND_ORDER_P_DELTA;

    static_analysis_settings.create(3);
    static_analysis_settings[3].analysis_type = static_analysis_settings.LARGE_DEFORMATIONS;
    if (RFEM)
    {
        static_analysis_settings[3].iterative_method_for_nonlinear_analysis = static_analysis_settings.NEWTON_RAPHSON;
        static_analysis_settings[3].max_number_of_iterations = 100;
        static_analysis_settings[3].number_of_load_increments = 10;
    }
    else
    {
        static_analysis_settings[3].max_number_of_iterations = 100;
        static_analysis_settings[3].number_of_load_increments = 10;
    }
    return true;
}

function createStandardStabilityAnalysisSettings()
{
  /*
  Create 3 standard Stability Analysis Settings
  Example:
    createStandardStabilityAnalysisSettings();
  */
  stability_analysis_settings.create(1);

  if (RFEM)
  {
    stability_analysis_settings[1].analysis_type = stability_analysis_settings.EIGENVALUE_METHOD;

    stability_analysis_settings.create(2);
    stability_analysis_settings[2].analysis_type = stability_analysis_settings.INCREMENTALY_METHOD_WITH_EIGENVALUE;

    stability_analysis_settings.create(3);
    stability_analysis_settings[3].analysis_type = stability_analysis_settings.INCREMENTALY_METHOD_WITHOUT_EIGENVALUE;
  }

  return true;
}

function createStandardModalAnalysisSettings()
{
    /*
    Create 5 standard Modal Analysis Settings (1,2 exists)
    Example:
        createStandardModalAnalysisSettings();
    */
    if (RSTAB)
    {
        return;
    }
    modal_analysis_settings.create(3);
    modal_analysis_settings[3].solution_method = modal_analysis_settings.METHOD_ROOT_OF_CHARACTERISTIC_POLYNOMIAL;

    modal_analysis_settings.create(4);
    modal_analysis_settings[4].solution_method = modal_analysis_settings.METHOD_SUBSPACE_ITERATION;

    if (PRERELEASE_MODE)
    {
        modal_analysis_settings.create(5);
        modal_analysis_settings[5].solution_method = modal_analysis_settings.METHOD_ICG_ITERATION;
    }

    return true;
}
function createStandardLoadCase(index, staticAnalysisSettings, action_category, name)
{
    /*
    Create Load cases (first is always self-weight)
    Example:
        createStandardLoadCase(int, int, int/str, str);
        createStandardLoadCase(1, 1, 1, "Self weight");
    */
    var load_case = load_cases.create(index);
    load_cases[index].name = name;
    load_cases[index].static_analysis_settings = staticAnalysisSettings;
    load_cases[index].action_category = action_category;
    if (index == 1)
    {
        load_cases[index].self_weight_active = true;
        load_cases[index].action_category = load_cases.ACTION_CATEGORY_PERMANENT_G;
    }

    return load_case;
}
function createStandardLoadCombination(index, staticAnalysisSettings, design_situation, name)
{
    /*
    Create Load combination
    Example:
        createStandardLoadCombination(int, int, int, str);
        createStandardLoadCombination(1, 1, 1, "Load combination 01");

    */
    var load_combination = load_combinations.create(index);
    load_combinations[index].static_analysis_settings = staticAnalysisSettings;
    load_combinations[index].design_situation = design_situation;
    if (name != "")
    {
        load_combinations[index].user_defined_name_enabled = true;
        load_combinations[index].name = name;
    }
    return load_combination;
}

function createStandardResultCombination(index, design_situation, name)
{
    /*
    Create Empty Result combination, by fist RC is activated tab Result combination
    Example:
        createStandardResultCombination(int, int, str);
        createStandardResultCombination(1, 1, "Result cobmination 1");

    */
    if (index == 1)
    {
        load_cases_and_combinations.result_combinations_active = true;
    }
    var result_combination = result_combinations.create(index);
    result_combinations[index].design_situation = design_situation;
    if (name != "")
    {
        result_combinations[index].user_defined_name_enabled = true;
        result_combinations[index].name = name;
    }

    return result_combination;
}

function createStandardDesignSituation(index, design_situation_type, name)
{
    /*
    Create Design Situation
    Example:
        createStandardDesignSituation(int, int/str, str);
        createStandardDesignSituation(1, design_situations.DESIGN_SITUATION_TYPE_EQU_PERMANENT_AND_TRANSIENT, "Design situation 1");
    */
    var design_situation = design_situations.create(index);
    design_situations[index].design_situation_type = design_situation_type;
    if (name != "")
    {
        design_situations[index].user_defined_name_enabled = true;
        design_situations[index].name = name;
    }
    return design_situation;
}

function createStandardImperfectionCase(index, imperfection_type, name)
{
    /*
    Create standard Imperfection Cases
    Example:
        createStandardImperfectionCase(int, int/reference item, str);
        createStandardImperfectionCase(1, 1, "Local Imperfections Only 1");
    */
    var imperfection_case = imperfection_cases.create(index);
    imperfection_cases[index].type = imperfection_type;
    if (name != "")
    {
        imperfection_cases[index].name = name;
    }
    return imperfection_case;
}
function createStandardLineMeshRefinement(index, lines, targetLenght)
{
    /*
    Create standard Line Mesh Refinement
    Example:
        createStandardLineMeshRefinement(int, str, int);
        createStandardLineMeshRefinement(1,'1,2-15', 0.1m);
    */
    if (RSTAB)
    {
        return;
    }
    var line_mesh_refinement = line_mesh_refinements.create(index);
    line_mesh_refinements[index].target_length = targetLenght;
    line_mesh_refinements[index].lines = lines;

    return line_mesh_refinement;
}

function createStandardSurfaceMeshRefinement(index, surfaces, targetLength)
{
    /*
    Create standard Surface Mesh Refinement
    Example:
        createStandardSurfaceMeshRefinement(int, str, int);
        createStandardSurfaceMeshRefinement(1,'1,2-15', 0.1m);
    */
    if (RSTAB)
    {
        return;
    }
    var surface_mesh_refinement = surface_mesh_refinements.create(index);
    surface_mesh_refinements[index].target_length = targetLength;
    surface_mesh_refinements[index].surfaces = surfaces;

    return surface_mesh_refinement;
}

function createStandardSolidMeshRefinement(index, solids, targetLength)
{
    /*
    Create standard Solid Mesh Refinement
    Example:
        createStandardSolidMeshRefinement(int, str, int);
        createStandardSolidMeshRefinement(1,'1,2-15', 0.1m);
    */
    if (RSTAB)
    {
        return;
    }
    var solid_mesh_refinement = solid_mesh_refinements.create(index);
    solid_mesh_refinements[index].target_length = targetLength;
    solid_mesh_refinements[index].solids = solids;

    return solid_mesh_refinement;
}

function createStandardMaterial(index, databaseName)
{
    /*
    Create material from database
    Example:
        createStandardMaterial(int, str);
        createStandardMaterial(1, 'S235');
    */
    var material = materials.create(index);
    materials[index].name = databaseName;

    return material;
}

function createStandardSection(index, material, databaseName)
{
    /*
    Create material from database
    Example:
        createStandardSection(int, int, str);
        createStandardSection(1, 1, 'IPE 80');
    */
    var section = sections.create(index);
    sections[index].material = material;
    sections[index].name = databaseName;

    return section;
}

function createStandardThickness(index, material, uniformThickness)
{
    /*
    Create uniform thickness
    Example:
        createStandardThickness(int, int, str or int);
        createStandardThickness(1, 1, 150mm);
    */
    if (RSTAB)
    {
        return;
    }
    var thickness = thicknesses.create(index);
    thickness.uniform_thickness = uniformThickness;
    thickness.material = material;

    return thickness;
}

function createStandardMember(index, nodes, section)
{
    /*
    Create standard member with section and (also with line)
    Example:
        createStandardMember(int, str, str);
        createStandardMember( 1, '1,2', '1');
    */
    if (RFEM)
    {
        var line = lines.create(index);
        lines[index].definition_nodes = nodes;
    }
    var member =  members.create(index);
    if (RFEM)
    {
        members[index].line = index;
    }
    else
    {
        var nodes = nodes.toString().split(',');
        members[index].node_start = nodes[0];
        members[index].node_end = nodes[1];
    }
    members[index].section_start = section;

    return {
        line: line,
        member: member
    }
}

function createStandardSurface(index, lines, thickness)
{
    /*
    Create standard surface with thickness on boundary lines
    Example:
        createStandardSurface(int, str, str);
        createStandardSurface(1, '4,10,9,11', '1');
    */
    if (RSTAB)
    {
        return;
    }
    var surface = surfaces.create(index);
    surfaces[index].boundary_lines = lines;
    surfaces[index].thickness = thickness;

    return surface;
}

function createStandardSolid(index, surfaces, material)
{
    /*
    Create standard solid with material on boundary surfaces
    Example:
        createStandardSolid(int, str, str);
        createStandardSolid(1, '6-12', '1');
    */
    if (RSTAB)
    {
        return;
    }
    var solid = solids.create(index);
    solids[index].boundary_surfaces = surfaces;
    solids[index].material = material;

    return solid;
}
function setStandardSettings()
{
    /*
    Create standard general settings
    Example:
        setStandardSettings();
    */
    STRESS_ANALYSIS.setActive(false);
    CONCRETE_DESIGN.setActive(false);
    STEEL_DESIGN.setActive(false);

    general.gravitational_acceleration = 10.0;

    general.tolerance_for_nodes = 0.0005;
    general.tolerance_for_lines = 0.0005;
    general.tolerance_for_surfaces = 0.0005;
    general.tolerance_for_directions = 0.0005;

    general.setLocalAxesOrientationZDown(true);
    general.setGlobalAxesOrientationZDown(true);

    return true;
}
function setActiveAllAddons(bool)
{
    /*
    Activate all addons
    Example:
        turnOnAllAddons(true);
    */
    STRESS_ANALYSIS.setActive(bool);
    CONCRETE_DESIGN.setActive(bool);
    STEEL_DESIGN.setActive(bool);
    STEEL_JOINTS.setActive(bool);
    if (typeof TIMBER_JOINTS == "object")
    {
        TIMBER_JOINTS.setActive(bool);
    }
    TIMBER_DESIGN.setActive(bool);
    MATERIAL_NONLINEAR_ANALYSIS.setActive(bool);
    STRUCTURE_STABILITY.setActive(bool);
    CONSTRUCTION_STAGES.setActive(bool);
    FORM_FINDING.setActive(bool);
    TORSIONAL_WARPING.setActive(bool);
    DYNAMIC_ANALYSIS.MODAL.setActive(bool);
    DYNAMIC_ANALYSIS.SPECTRAL.setActive(bool);
	if (PRERELEASE_MODE)
	{
		DYNAMIC_ANALYSIS.TIME_HISTORY.setActive(bool);
	}
    return true;
}

function createOrMergeNode(newCoordinates, options)
{
    /*
    Create New node, if none exist on same coordinates
    Example:
        copyNode(10, [1,0,0]);
     */
    for(var i = 0; i <= nodes.lastId(); i++)
    {
       if (nodes.exist(i)==true){
           var coordinates2 = nodes[i].coordinates;
           if (coordinates2.x.toFixed(2) == newCoordinates.x.toFixed(2) && coordinates2.y.toFixed(2) == newCoordinates.y.toFixed(2) && coordinates2.z.toFixed(2) == newCoordinates.z.toFixed(2)) {return nodes[i];}//return node;
       }
    }
    var newNode = nodes.create(nodes.lastId() + 1, options);
    newNode.coordinates = newCoordinates;
    return newNode;
}

function copyNode(index, vector, options)
{
    /*
    Create Copy of node
    Example:
        copyNode(10, [1,0,0]);
     */
    var coordinates = nodes[index].coordinates;
    var newNode = createOrMergeNode($V(coordinates.x+vector[0],coordinates.y+vector[1],coordinates.z+vector[2]), options);
    return newNode;
}

function copyLine(index, vector, options)
{
    /*
    Create Copy of line (with nodes)
    Example:
        copyLine(2, [1,0,0]);
     */
    var assignedNodes = lines[index].definition_nodes;
    var arrNodes = [];
    for (var i = 0; i < assignedNodes.length; i++)
    {
        var nodeToAssign = copyNode(assignedNodes[i].id, vector);
        arrNodes.push(nodeToAssign.id);
    }


    for(var i = 0; i <= lines.lastId(); i++)
    {
        if (lines.exist(i)==true)
        {
            var copiedNodes = [];
            var copiedLineWithNodes = lines[i].definition_nodes;
            for (var e = 0; e < copiedLineWithNodes.length; e++)
            {
                copiedNodes.push(copiedLineWithNodes[e].id);
            }
            if (arrNodes.sort(function(a, b){return a-b}).join('') === copiedNodes.sort(function(a, b){return a-b}).join('')) {return lines[i];}
        }
    }
    arrNodes = arrNodes.join(",");
    newLine = createLine(lines.lastId() + 1, arrNodes, options);
    return newLine;
}

function copyMember(index, vector, options)
{
    /*
    Create Copy of member (with line, nodes)
    Examples:
        copyMember(2, [1,0,0]);
        copyMember(1, [0,-1,0],{section_start:1});
     */
    if (RFEM)
    {
        var copiedLine = members[index].line;
    }
    if (RSTAB)
    {
        var assignedNodes = members[index].nodes;
        var arrNodes = [];
        for (var i = 0; i < assignedNodes.length; i++)
        {
            var nodeToAssign = copyNode(assignedNodes[i].id, vector);
            arrNodes.push(nodeToAssign.id);
        }
    }
    for(var i = 0; i <= members.lastId(); i++)
    {
        if (members.exist(i)==true)
        {
            if (RFEM)
            {
                var newLine = copyLine(copiedLine.id, vector);
                if (members[i].line.id==newLine.id)
                {
                    return members[i];
                }
            }
            if (RSTAB)
            {
                var copiedNodes = [];
                var copiedMemberWithNodes = members[i].nodes;
                for (var e = 0; e < copiedMemberWithNodes.length; e++)
                {
                    copiedNodes.push(copiedMemberWithNodes[e].id);
                }
                if (arrNodes.sort(function(a, b){return a-b}).join('') === copiedNodes.sort(function(a, b){return a-b}).join('')) {return members[i];}//return member if exists
            }
        }
    }
    if (RFEM)
    {
        var newMember = createMember2(members.lastId()+1, newLine, options);
    }
    if (RSTAB)
    {
        var newMember = createMember(members.lastId()+1, arrNodes.join(','), options);
    }
    return newMember;
}

function copySurface(index, vector, options)
{
    /*
    Create Copy of Surface (with nodes and lines)
    Examples:
        copySurface(2, [1,0,0]);
        copySurface(7, [0,-1,0],{thickness:1});
     */
    if (RSTAB)
    {
        return;
    }
    var assignedLines = surfaces[index].boundary_lines;
    var assignedType = surfaces[index].type;
    var arrLines = [];

    for (var i = 0; i < assignedLines.length; i++) {
        var linesToAssign = copyLine(assignedLines[i].id, vector);
        arrLines.push(linesToAssign.id);
    }

    for(var i = 0; i <= surfaces.lastId(); i++)
    {
        if (surfaces.exist(i)==true)
        {
            var copiedSurfaces = [];
            var copiedcopiedSurfaceWithLines = surfaces[i].boundary_lines;
            for (var e = 0; e < copiedcopiedSurfaceWithLines.length; e++)
            {
                copiedSurfaces.push(copiedcopiedSurfaceWithLines[e].id);
            }
            if (arrLines.sort(function(a, b){return a-b}).join('') === copiedSurfaces.sort(function(a, b){return a-b}).join('')) {return surfaces[i];}
        }
    }
    arrLines = arrLines.join(",");
    var newSurface = createSurface(surfaces.lastId() + 1, arrLines, options);
    newSurface.type = assignedType;
    return newSurface;
}

function copySolid(index, vector, options)
{
    /*
    Create Copy of Solid (with nodes and lines and surfaces)
    Example:
        copySolid(2, [1,0,0]);
        copySolid(1, [0,-1,0],{material:2});
     */
    var assignedSurfaces = solids[index].boundary_surfaces;
    var arrSurfaces = [];

    for (var i = 0; i < assignedSurfaces.length; i++) {
        var surfacesToAssign = copySurface(assignedSurfaces[i].id, vector);
        arrSurfaces.push(surfacesToAssign.id);
    }

    arrSurfaces = arrSurfaces.join(",");
    var newSolid = createSolid(solids.lastId() + 1, arrSurfaces, options);
    return newSolid;
}

function createNodeOnLine(onLineId, relativeDistanceFromStart)
{
    if (RFEM)
    {
        var line = lines[onLineId];
        if (line)
        {
            var nodeRow = line.nodes_on_line_assignment[line.nodes_on_line_assignment.row_count()];
            nodeRow[4] = relativeDistanceFromStart;
            return nodes[nodeRow[2]];
        }
    }
    return null;
}
/*
Define base func
*/
if (!String.prototype.format) {
  String.prototype.format = function() {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function(match, number) {
      return typeof args[number] != 'undefined'
        ? args[number]
        : match
      ;
    });
  };
}


function createSteelJoint(index, nodes, options)
{
    /*
    Create steel joint on node.
    Example:
        createSteelJoint(int, str);
        createSteelJoint(1, '1');
        createSteelJoint(6, '1,5', {name: 'Main frame joint', comment: 'first etape'});
    */
    var joint =  steel_joints.create(index);
    if (nodes != '')
    {
       joint.nodes = nodes;
    }
    if (!isObjectEmpty(options))
    {
        //User settings options
        Object.keys(options).map(function(key){return steel_joints[index][key] = options[key]});
    }
    return joint;
}

function createNodeOnMember(memberIndex, lenghtFromStart)
{
    var member = members[memberIndex];
    if (member)
    {
        var nodeRow = member.nodes_on_member_assignment[member.nodes_on_member_assignment.row_count()];
        nodeRow[4] = lenghtFromStart;
        return nodes[nodeRow[2]];
    }
    return null;
}

function createFormulaParameterTypeValue(index, name, unitGroup, value) {

    /* Create new formula parameter type value
    Example:
        createFormulaParameterTypeValue(int, str, int/str, double/string);
        createFormulaParameterTypeValue(2, "Zatížení", "Forces", 10.5);
    */
    var formulas = global_parameters.create(index);
    formulas.name = name;
    formulas.unit_group = unitGroup;
	formulas.definition_type = "Value";
    formulas.value = value;

    return formulas;
}

function createFormulaParameterTypeFormula(index, name, unitGroup, formula) {

    /* Create new formula parameter type formula
    Example:
        createFormulaParameterTypeFormula(int, str, int/str, int/str);
        createFormulaParameterTypeFormula(2, "Zatížení", "Forces", "=2*5");
    */
    var formulas = global_parameters.create(index);
    formulas.name = name;
    formulas.unit_group = unitGroup;
	formulas.definition_type = "Formula";
    formulas.formula = formula;

    return formulas;
}
