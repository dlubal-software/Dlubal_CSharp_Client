run("clearAll.js");
include("global.js");

function create_line_and_member(lineNo, node_a, node_b, cross_section)
{
    if (RFEM)
    {
        var line = lines.create(lineNo)
        line.definition_nodes = node_a + "," + node_b;
        var member = members.create(lineNo);
        member.line = lineNo;
        member.cross_section_start = cross_section;
        member.type = members.TYPE_BEAM;
        return {
            line: line,
            member: member
        };
    }

    if (RSTAB)
    {
        var memberNo = lineNo;
        var member = members.create(memberNo);
        member.node_start = node_a;
        member.node_end = node_b;
        member.cross_section_start = cross_section;
        member.type = members.TYPE_BEAM;
        return {
            member: member
        };
    }
}

STRUCTURE_STABILITY.setActive(true);

//GEOMETRY
var numOfFrames = 5;

    //Geometry of frame
var L = 12; //span
var H1 = 3; //height of columns
var H2 = 1.5; //height of gable
var H3 = 2.5; //height of console
var L1 = 0.3; //left console
var L2 = 0.3; //right console
var L3 = 5; //length between frames

    //Loading - Live and wind
var f_g = 150; //N/m2
var f_q = 750; //N/m2
var f_w = 500; //N/m2
var B = L3;

//USER - COORDINATE SYSTEM
createCoordinateSystem(2, $V([0.1, 0.3, 1.5]), {comment:"Vlastni SS", u_axis_point_coordinates: $V([2.3, 0.3, 3.4]),uw_plane_point_coordinates: $V([-2, 3, 2.5])});
coordinate_systems[2].user_defined_name_enabled = true;
coordinate_systems[2].name = "Vlastni SS";

createStandardStaticAnalysisSettings();
createStandardDesignSituation(1, design_situations.DESIGN_SITUATION_TYPE_EQU_PERMANENT_AND_TRANSIENT /* ULS (EQU) - Permanent and transient */, "DS 1");

//LOAD CASES
createStandardLoadCase(1, 1, load_cases.ACTION_CATEGORY_PERMANENT_G, "Self weight");
createStandardLoadCase(2, 2, load_cases.ACTION_CATEGORY_IMPOSED_LOADS_CATEGORY_H_ROOFS_QI_H, "Live load");
createStandardLoadCase(3, 2, load_cases.ACTION_CATEGORY_WIND_QW, "Wind load");
createStandardLoadCase(4, 2, load_cases.ACTION_CATEGORY_WIND_QW, "Wind load 2");
createStandardLoadCase(6, 2, load_cases.ACTION_CATEGORY_SNOW_ICE_LOADS_H_LESS_OR_EQUAL_TO_1000_M_QS, "Snow load");
createStandardLoadCase(8, 2, load_cases.ACTION_CATEGORY_PERMANENT_G, "Other permanent load");


createStandardLoadCase(5, 1, load_cases.ACTION_CATEGORY_PERMANENT_IMPOSED_GQ, "Stability - linear");

//assign stability
load_cases[5].self_weight_active = true;
load_cases[5].calculate_critical_load = true;
load_cases[5].stability_analysis_settings = 1;

createStandardLoadCase(7, 2, load_cases.ACTION_CATEGORY_PERMANENT_IMPOSED_GQ, "Imperfections");

//assign imperfections
load_cases[7].consider_imperfection = true;
load_cases[7].imperfection_case = 1;

//LOAD COMBINATIONS
var LC = createStandardLoadCombination(1, 2, 1, "Load combination 1");
LC.items[1].factor = 1.35;
LC.items[1].load_case = 1;
LC.items[2].factor = 1.5;
LC.items[2].load_case = 2;

var LC = createStandardLoadCombination(2, 2, 1, "Load combination 2");
LC.items[1].factor = 1.35;
LC.items[1].load_case = 1;
LC.items[2].factor = 1.5;
LC.items[2].load_case = 3;

var LC = createStandardLoadCombination(3, 2, 1, "Load combination 3");
LC.items[1].factor = 1.35;
LC.items[1].load_case = 1;
LC.items[2].factor = 1.5;
LC.items[2].load_case = 4;

var LC = createStandardLoadCombination(4, 2, 1, "Characteristic SLS");
LC.items[1].factor = 1.0;
LC.items[1].load_case = 1;
LC.items[2].factor = 1.0;
LC.items[2].load_case = 8;


//RESULT COMBINATION
var RC = createStandardResultCombination(1, 1, "Result combination 1");
RC.items[1].case_object_item = load_combinations[1];
RC.items[1].case_object_factor = 1.00;
RC.items[1].case_object_load_type = result_combinations.LOAD_TYPE_PERMANENT;
RC.items[1].operator = result_combinations.OPERATOR_AND;
RC.items[2].case_object_item = load_combinations[2];
RC.items[2].case_object_factor = 1.0;
RC.items[2].case_object_load_type = result_combinations.LOAD_TYPE_PERMANENT;
RC.items[2].operator = result_combinations.OPERATOR_AND;
RC.items[3].case_object_item = load_combinations[3];
RC.items[3].case_object_factor = 1.0;
RC.items[3].case_object_load_type = result_combinations.LOAD_TYPE_PERMANENT;
RC.items[3].operator = result_combinations.OPERATOR_OR;
RC.items[4].case_object_item = load_cases[2];
RC.items[4].case_object_factor = 1.0;
RC.items[4].case_object_load_type = result_combinations.LOAD_TYPE_PERMANENT;
RC.items[4].operator = result_combinations.OPERATOR_OR;
RC.items[5].case_object_item = load_cases[3];
RC.items[5].case_object_factor = 1.0;
RC.items[5].case_object_load_type = result_combinations.LOAD_TYPE_PERMANENT;
RC.items[5].operator = result_combinations.OPERATOR_AND;
RC.items[6].case_object_item = load_cases[4];
RC.items[6].case_object_factor = 1.0;
RC.items[6].case_object_load_type = result_combinations.LOAD_TYPE_TRANSIENT;

RC = createStandardResultCombination(2, 1, "Result combination 2");
RC.items[1].case_object_item = load_combinations[1];
RC.items[1].case_object_factor = 1.00;
RC.items[1].case_object_load_type = result_combinations.LOAD_TYPE_PERMANENT;

//STABILITY ANALYSIS
stability_analysis_settings.erase(1);
stability_analysis_settings.erase(2);
createStandardStabilityAnalysisSettings();

//IMPERFECTIONS
createStandardImperfectionCase(1, 1, "Local Imperfections Only 1");


//MODEL PARAMETERS
// Material
createStandardMaterial(1, 'S235 | EN 1993-1-1:2005-05');

// Section
createStandardSection(1, 1,'HEB 220');
createStandardSection(2, 1,'IPE 240');
createStandardSection(3, 1,'IPE 100');
createStandardSection(4, 1,'L 20x20x3');

//Nodal supports
createNodalSupport( 1, '', ["inf", "inf", "inf", 0, "inf", "inf"]);


//GENERATE CONSTRUCTION AND LOADING
var numNodes= 9 *numOfFrames;

    //nodes
for (var i = 1; i < numNodes+1; ++i)
{
    nodes.create(i);
}
    //nodes - coordinates
for (var i = 1; i < numOfFrames+1; ++i)
{
    var dy = L3 * (i-1);
    var dNo = (i-1) * 9;
    nodes[1+dNo].coordinates = $V(0,dy,0);
    nodes[2+dNo].coordinates = $V(0, dy, -H3);
    nodes[3+dNo].coordinates = $V(0, dy, -H1);
    nodes[4+dNo].coordinates = $V(L / 2, dy, -H1 - H2);
    nodes[5+dNo].coordinates = $V(L, dy, -H1);
    nodes[6+dNo].coordinates = $V(L, dy, -H3);
    nodes[7+dNo].coordinates = $V(L, dy, 0);
    nodes[8+dNo].coordinates = $V(L1, dy, -H3);
    nodes[9+dNo].coordinates = $V(L - L2, dy, -H3);
}
    //simple frames
for (var i = 1; i < numOfFrames+1; ++i)
{
    var dNoLine = (i-1) * 8;
    var dNo = (i-1) * 9;
    create_line_and_member(1+dNoLine, 1+dNo, 2+dNo,cross_sections[1]);
    create_line_and_member(2+dNoLine, 2+dNo, 3+dNo,cross_sections[1]);
    create_line_and_member(3+dNoLine, 3+dNo, 4+dNo,cross_sections[2]);
    create_line_and_member(4+dNoLine, 4+dNo, 5+dNo,cross_sections[2]);
    create_line_and_member(5+dNoLine, 5+dNo, 6+dNo,cross_sections[1]);
    create_line_and_member(6+dNoLine, 2+dNo, 8+dNo,cross_sections[2]);
    create_line_and_member(7+dNoLine, 6+dNo, 9+dNo,cross_sections[2]);
    create_line_and_member(8+dNoLine, 6+dNo, 7+dNo,cross_sections[1]);


}

    //longtitudial beams
for (var i = 1; i < numOfFrames; ++i)
{
    var dNoLine = (i-1) * 5;
    var startLine = numOfFrames * 8 + 1;
    var dNo = (i-1) * 9;
    create_line_and_member(startLine+dNoLine, 2+dNo, 11+dNo,cross_sections[3]);
    create_line_and_member(startLine+1+dNoLine, 6+dNo, 15+dNo,cross_sections[3]);
    create_line_and_member(startLine+2+dNoLine, 4+dNo, 13+dNo,cross_sections[3]);
    create_line_and_member(startLine+3+dNoLine, 5+dNo, 14+dNo,cross_sections[3]);
    create_line_and_member(startLine+4+dNoLine, 3+dNo, 12+dNo,cross_sections[3]);
}
    // bracing system
for (var i = 1; i < 2; ++i)
{
    var dNoLine = (i-1) * 8;
    var startLine = numOfFrames * 8 + 1 + 5 * (numOfFrames-1) -3 ;
    var dNo = (i-1) * 9;
    create_line_and_member(startLine+3+dNoLine, 7+dNo, 14+dNo,cross_sections[4]);
    create_line_and_member(startLine+4+dNoLine, 5+dNo, 16+dNo,cross_sections[4]);
    create_line_and_member(startLine+5+dNoLine, 5+dNo, 13+dNo,cross_sections[4]);
    create_line_and_member(startLine+6+dNoLine, 14+dNo, 4+dNo,cross_sections[4]);
    create_line_and_member(startLine+7+dNoLine, 13+dNo, 3+dNo,cross_sections[4]);
    create_line_and_member(startLine+8+dNoLine, 12+dNo, 4+dNo,cross_sections[4]);
    create_line_and_member(startLine+9+dNoLine, 1+dNo, 12+dNo,cross_sections[4]);
    create_line_and_member(startLine+10+dNoLine, 3+dNo, 10+dNo,cross_sections[4]);
    members[startLine+3+dNoLine].type = members.TYPE_TENSION;
    members[startLine+4+dNoLine].type = members.TYPE_TENSION;
    members[startLine+5+dNoLine].type = members.TYPE_TENSION;
    members[startLine+6+dNoLine].type = members.TYPE_TENSION;
    members[startLine+7+dNoLine].type = members.TYPE_TENSION;
    members[startLine+8+dNoLine].type = members.TYPE_TENSION;
    members[startLine+9+dNoLine].type = members.TYPE_TENSION;
    members[startLine+10+dNoLine].type = members.TYPE_TENSION;
}


//assign supports
for (var i = 1; i < numOfFrames+1; ++i)
{
    var dNo = (i-1) * 9;
    nodes[1+dNo].support = nodal_supports[1];
    nodes[7+dNo].support = nodal_supports[1];
}

//assign load
    //live load LC2
var member_load_ = load_cases[2].member_loads;
var member_load = member_load_.create(1);

var num = [];
for (var i = 1; i < numOfFrames+1; ++i)
{
    var dNo = (i-1) * 8;
    num.push(3+dNo);
    num.push(4+dNo);
}
member_load.members = num.toString();
member_load.load_distribution = member_loads.LOAD_DISTRIBUTION_UNIFORM;
member_load.load_direction = member_loads.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE;
member_load.magnitude = f_q * B;

//imperfections
createMemberImperfection(1, 7, 1, "1,9,17,25,33", 5mm)

    //crane
var num = [];
for (var i = 1; i < numOfFrames+1; ++i)
{
    var dNo = (i-1) * 9;
    num.push(8+dNo);
    num.push(9+dNo);
}
var nodal_load = load_cases[2].nodal_loads.create(1);
nodal_load.load_type = nodal_loads.LOAD_TYPE_COMPONENTS;
nodal_load.components_force = Vector.create(0, 0, 10000);
nodal_load.nodes = num.toString();

    //wind load LC3
var member_load_ = load_cases[3].member_loads;
var member_load = member_load_.create(1);

var num = [];
for (var i = 1; i < numOfFrames+1; ++i)
{
    var dNo = (i-1) * 8;
    num.push(1+dNo);
    num.push(2+dNo);
    num.push(5+dNo);
    num.push(8+dNo);
}
member_load.members = num.toString();
member_load.load_distribution = member_loads.LOAD_DISTRIBUTION_UNIFORM;
member_load.load_direction = member_loads.LOAD_DIRECTION_GLOBAL_X_OR_USER_DEFINED_U_TRUE;
member_load.magnitude = f_w * B;

    //wind load LC4
var member_load_ = load_cases[4].member_loads;
var member_load = member_load_.create(1);
member_load.members = "3,4";
member_load.load_distribution = member_loads.LOAD_DISTRIBUTION_UNIFORM;
member_load.load_direction = member_loads.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE;
member_load.magnitude = f_w * L/2;


var member_load = member_load_.create(2);

var num = [];
for (var i = 2; i < numOfFrames + 1; ++i)
{
    var dNo = (i-1) * 8;
    num.push(1+dNo);
    num.push(2+dNo);
    num.push(3+dNo);
    num.push(4+dNo);
    num.push(5+dNo);
    num.push(8+dNo);
}
member_load.members = num.toString();
member_load.load_distribution = member_loads.LOAD_DISTRIBUTION_UNIFORM;
member_load.load_direction = member_loads.LOAD_DIRECTION_GLOBAL_Y_OR_USER_DEFINED_V_TRUE;
member_load.magnitude = f_w * 0.015 ;

    //live load LC8 other permanent load
var member_load_ = load_cases[8].member_loads;
var member_load = member_load_.create(1);

var num = [];
for (var i = 1; i < numOfFrames+1; ++i)
{
    var dNo = (i-1) * 8;
    num.push(3+dNo);
    num.push(4+dNo);
}
member_load.members = num.toString();
member_load.load_distribution = member_loads.LOAD_DISTRIBUTION_UNIFORM;
member_load.load_direction = member_loads.LOAD_DIRECTION_GLOBAL_Z_OR_USER_DEFINED_W_TRUE;
member_load.magnitude = f_g * B;
