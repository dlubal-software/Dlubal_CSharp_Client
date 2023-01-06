run("clearAll.js");
include("global.js");
CONCRETE_DESIGN.setActive(true);

//Geometry
var t = 120mm; //thickness

//USER - COORDINATE SYSTEM
createCoordinateSystem(2, $V([0.1, 0.3, 1.5]), {comment:"Vlastni SS", u_axis_point_coordinates: $V([2.3, 0.3, 3.4]),uw_plane_point_coordinates: $V([-2, 3, 2.5])});
coordinate_systems[2].user_defined_name_enabled = true;
coordinate_systems[2].name = "Vlastni SS";

//LOAD CASES
createStandardStaticAnalysisSettings();

var LC = load_cases.create(1);
LC.name = "Self weight";
LC.static_analysis_settings = 1;
LC.self_weight_active = true;
LC.action_category = load_cases.ACTION_CATEGORY_PERMANENT_G;

var LC = load_cases.create(2);
LC.name = "Live load";
LC.static_analysis_settings = 2;
LC.action_category = load_cases.ACTION_CATEGORY_IMPOSED_LOADS_CATEGORY_H_ROOFS_QI_H;

//RESULT COMBINATION
var RC = createStandardResultCombination(1, 1, "Result combination 1");
RC.items[1].case_object_item = load_combinations[1];
RC.items[1].case_object_factor = 1.0;
result_combinations[1].combination_type="Superposition";

//LOAD COMBINATIONS
var LC = load_combinations.create(1);
LC.user_defined_name_enabled = true;
LC.name = "Load combination 1";
LC.static_analysis_settings = 2;
LC.design_situation = 1;
LC.items[1].factor = 1.35;
LC.items[1].load_case = 1;
LC.items[2].factor = 1.5;
LC.items[2].load_case = 2;


//MODEL PARAMETERS
// Material
createStandardMaterial(1, 'C12/15 | DIN 1045-1:2008-08');
createStandardMaterial(2, 'C20/25 | DIN 1045-1:2008-08');
createStandardMaterial(3, 'B550S(A) | EN 1992-1-1:2004/A1:2014');
createStandardMaterial(4, 'B550M(A) | EN 1992-1-1:2004/A1:2014');

// Section
createStandardSection(1, 1, 'R_M1 200/400');
sections[1].shear_stiffness_deactivated = true;
createStandardSection(2, 1, 'R_M1 200/200');
sections[1].shear_stiffness_deactivated = true;

// Thicknesses
createStandardThickness(1, 2, t);

//Mesh refinement
createStandardLineMeshRefinement(1, '4', 0.1m);
createStandardSurfaceMeshRefinement(1, '3', 0.8m);
createStandardSolidMeshRefinement(1, '', 0.01m);

//GENERATE GEOMETRY
createNode(1, $V([0, 0, -4]));
createNode(2, $V([9.5, 0, -4]));
createNode(3, $V([9.5, 6, -4]));
createNode(4, $V([0, 6, -4]));

createNode(6, $V([5, 2, -4]));
createNode(7, $V([7, 2, -4]));
createNode(8, $V([7, 4, -4]));
createNode(9, $V([5, 4, -4]));
createNode(10, $V([0, 6, 0]));
createNode(11, $V([0, 0, 0]));
createNode(12, $V([6, 6, -4]));
createNode(13, $V([9.5, 0, 0]));
createNode(14, $V([9.5, 6, 0]));

createNode(16, $V([6, 6, 0]));
createNode(17, $V([0, 5, -3]));
createNode(18, $V([0, 3, -3.52]));
createNode(19, $V([0, 1, -3]));
createNode(20, $V([0, 5.456, 0]));
createNode(21, $V([0, 0.588, 0]));
//createNode(22, $V([9.5, 3, 1]));
//createNode(23, $V([9.5, 3, 3]));
//createNode(24, $V([9.5, 2.506, 2.02]));
//createNode(25, $V([9.5, 1.494, 1]));
//createNode(26, $V([9.5, 2.506, 2]));
//createNode(27, $V([9.5, 1.494, 3]));
//createNode(28, $V([9.5, 1, 2]));
//createNode(29, $V([9.5, 4.494, 1]));
//createNode(30, $V([9.5, 4.494, 3]));
//createNode(31, $V([9.5, 4, 2]));


createLine( 1, '1,2');
createLine( 2, '11,13');
createLine( 3, '12,3');
createLine( 4, '4,1');
createLine( 5, '17,20');
createLine( 6, '2,3');
createLine( 7, '6,7');
createLine( 8, '7,8');
createLine( 9, '8,9');
createLine( 10, '9,6');
createLine( 11, '4,10');
createLine( 12, '10,20');
createLine( 13, '11,1');
createLine( 14, '2,13');
if (RFEM)
{
    createLine( 15, '17,19', {type: lines.TYPE_ARC, arc_control_point: $V([0, 3, -3.520])});
}
createLine( 16, '13,14');
createLine( 17, '3,14');
createLine( 18, '4,12');
createLine( 19, '12,16');
// Elipse openings are not generating yet
//createLine( 20, '23,22', {type: lines.TYPE_ELLIPSE, ellipse_control_point: $V([9.5, 2.506, 2])});
createLine( 21, '20,21');
createLine( 22, '21,11');
createLine( 23, '21,19');
//createLine( 24, '27,25', {type: lines.TYPE_ELLIPSE, ellipse_control_point: $V([9.5, 1, 2])});
//createLine( 25, '30,29', {type: lines.TYPE_ELLIPSE, ellipse_control_point: $V([9.5, 4, 2])});

if (RFEM)
{
    createMember2(1, '18', {section_start:1, member_eccentricity_start:1, member_eccentricity_end:1});
    createMember2(2, '3', {section_start:1, member_eccentricity_start:1, member_eccentricity_end:1});
    createMember2(3, '19', {section_start:2});
}
else
{
    createMember(1, '4,12', {section_start:1, member_eccentricity_start:1, member_eccentricity_end:1});
    createMember(2, '12,3', {section_start:1, member_eccentricity_start:1, member_eccentricity_end:1});
    createMember(3, '12,16', {section_start:2});
}

createSurface( 1, '4,13,22,21,12,11', {thickness:1});
createSurface( 2, '6,14,16,17', {thickness:1});
createSurface( 3, '2,14,1,13', {thickness:1});
createSurface( 4, '1,6,3,18,4', {thickness:1});

createOpening(1, '5,15,21,23');
//createOpening(2, '20');
//createOpening(3, '25');
//createOpening(4, '24');
createOpening(5, '7-10');

createNodalSupport( 1, '16', ["inf", "inf", "inf", 0, 0, "inf"]);
createLineSupport( 1, '2,12,16,22', ["inf", "inf", "inf", 0, 0, "inf"]);

createMemberEccentricity(1, [0,0,0],{specification_type:member_eccentricities.TYPE_RELATIVE_AND_ABSOLUTE, vertical_section_alignment: member_eccentricities.ALIGN_TOP, horizontal_section_alignment: member_eccentricities.ALIGN_MIDDLE})

//GENERATE LOADING
createSurfaceLoad( 1, 2, '4', 0.75kN/m^2);
createNodalLoad( 1, 2, '12', $V([1kN, 2kN, 3kN]));
createLineLoad( 1, 2, '15', 1.25kN/m);
createMemberLoad( 1, 2, '1,2', 1.25kN/m);

//DESIGN SITUATION
createStandardDesignSituation(1, design_situations.DESIGN_SITUATION_TYPE_STR_PERMANENT_AND_TRANSIENT_6_10, "DS 1");

//ADDON PROPERTIES
CONCRETE_DESIGN.design_situations[1].to_design = true;
CONCRETE_DESIGN.materials[1].to_design = true;
CONCRETE_DESIGN.objects_to_design.members_design_all = true;
general.current_standard_for_concrete_design = general.NATIONAL_ANNEX_AND_EDITION_EN_1992_CEN_2014_11;

function setMemberReinforcement(member)
{
    // longitudinal item
    members[member].concrete_longitudinal_reinforcement_items[1].material = materials[3];
    members[member].concrete_longitudinal_reinforcement_items[1].rebar_type = members.REBAR_TYPE_SYMMETRICAL;
    members[member].concrete_longitudinal_reinforcement_items[1].bar_count_symmetrical = 6;
    members[member].concrete_longitudinal_reinforcement_items[1].bar_diameter_symmetrical = 8mm;
    members[member].concrete_longitudinal_reinforcement_items[1].anchorage_start_anchor_type = members.ANCHORAGE_TYPE_NONE;
    members[member].concrete_longitudinal_reinforcement_items[1].anchorage_end_anchor_type = members.ANCHORAGE_TYPE_NONE;
    // shear span
    members[member].concrete_shear_reinforcement_spans[1].material = materials[4];
    members[member].concrete_shear_reinforcement_spans[1].stirrup_type = members.STIRRUP_TYPE_TWO_LEGGED_CLOSED_HOOK_135;
    members[member].concrete_shear_reinforcement_spans[1].stirrup_distances = 300mm;
    members[member].concrete_shear_reinforcement_spans[1].stirrup_diameter = 8mm;
}

setMemberReinforcement(1);
setMemberReinforcement(2);
setMemberReinforcement(3);

members[1].concrete_durability = 1;
members[2].concrete_durability = 1;
members[3].concrete_durability = 1;

function createSurfaceReinforcementTemplate()
{
    if (RSTAB)
    {
        return;
    }
    var reinforcement = surface_reinforcements.create();
    reinforcement.material = 3;
    reinforcement.additional_transverse_reinforcement_enabled = true;
    reinforcement.additional_rebar_diameter = 0.012;
    reinforcement.additional_rebar_spacing = 0.130;
    return reinforcement;
}

createSurfaceReinforcementTemplate();

function setSurfaceReinforcementTemplate(surface, reinforcement)
{
    if (RSTAB)
    {
        return;
    }
    surfaces[surface].surface_reinforcements = reinforcement;
    surfaces[surface].concrete_durability_top = 1;
    surfaces[surface].concrete_durability_bottom = 1;
    surfaces[surface].reinforcement_direction_top = 1;
    surfaces[surface].reinforcement_direction_bottom = 1;
}

setSurfaceReinforcementTemplate(1, 1);
setSurfaceReinforcementTemplate(2, 1);
setSurfaceReinforcementTemplate(3, 1);
setSurfaceReinforcementTemplate(4, 1);
