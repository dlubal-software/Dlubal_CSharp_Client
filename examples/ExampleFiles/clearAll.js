
if (RFEM) {
    MATERIAL_NONLINEAR_ANALYSIS.setActive(false);
    CONSTRUCTION_STAGES.setActive(false);
    TIME_DEPENDENT.setActive(false);
    FORM_FINDING.setActive(false);
    WIND_SIMULATION.setActive(false);
    MASONRY_DESIGN.setActive(false);
    BUILDING_MODEL.setActive(false);
    GEOTECHNICAL_ANALYSIS.setActive(false);
    if (PRERELEASE_MODE)
    {
        CUTTING_PATTERNS.setActive(false);
    }
}

if (RFEM || RSTAB) {
    STRESS_ANALYSIS.setActive(false);
    CONCRETE_DESIGN.setActive(false);
    STEEL_DESIGN.setActive(false);
    if (typeof STEEL_JOINTS == "object")
    {
        STEEL_JOINTS.setActive(false);
    }
    if (typeof TIMBER_JOINTS == "object") {
        TIMBER_JOINTS.setActive(false);
    }
    TIMBER_DESIGN.setActive(false);
    ALUMINUM_DESIGN.setActive(false);
    CRANEWAY_DESIGN.setActive(false);
    DYNAMIC_ANALYSIS.MODAL.setActive(false);
    DYNAMIC_ANALYSIS.SPECTRAL.setActive(false);
    if (PRERELEASE_MODE)
    {
        DYNAMIC_ANALYSIS.TIME_HISTORY.setActive(false);
    }
    STRUCTURE_STABILITY.setActive(false);
    TORSIONAL_WARPING.setActive(false);

    general.gravitational_acceleration = 10.0;
    general.tolerance_for_directions = 0.0005;
    general.tolerance_for_lines = 0.0005;
    general.tolerance_for_nodes = 0.0005;
    general.tolerance_for_surfaces = 0.0005;

    general.setLocalAxesOrientationZDown(true);
    general.setGlobalAxesOrientationZDown(true);
	general.terrain.type = general.NO_TERRAIN

    load_cases_and_combinations.activate_combination_wizard_and_classification = true;
    load_cases_and_combinations.current_standard_for_combination_wizard = general.NATIONAL_ANNEX_AND_EDITION_EN_1990_CEN_2010_04;
    load_cases_and_combinations.activate_combination_wizard = false;
}

else if (RSECTION) {
    general.tolerance_for_directions = 0.0005;
    general.tolerance_for_lines = 0.0005;
    general.tolerance_for_points = 0.0005;
}

if (RFEM)
{
    var containers = [
        lines,
        nodes,
        line_sets,
        materials,
        surfaces,
        solids,
        openings,
        nodal_supports,
        line_supports,
        surface_supports,
        surface_eccentricities,
        surface_stiffness_modifications,
        surfaces_contact_types,
        surfaces_contacts,
        rigid_links,
        line_hinges,
        line_welded_joints,
        thicknesses,
        sections,
        member_hinges,
        member_eccentricities,
        member_stiffness_modifications,
        member_result_intermediate_points,
        members,
        member_supports,
        member_nonlinearities,
        member_sets,
        member_definable_stiffnesses,
        design_supports,
        steel_member_shear_panels,
        steel_member_rotational_restraints,
        aluminum_member_shear_panels,
        aluminum_member_rotational_restraints,
        member_loads_from_area_load,
        member_loads_from_free_line_load,
        object_selections,
        snow_loads,
        wind_loads,
        surface_sets,
        solid_sets,
        intersections,
        structure_modifications,
        nodal_mesh_refinements,
        line_mesh_refinements,
        surface_mesh_refinements,
        surface_results_adjustments,
        solid_mesh_refinements,
        solid_gases,
        solid_contacts,
        steel_joints,
        timber_joints,
        result_sections,
        imperfection_cases,
        construction_stages,
        structure_modifications,
        load_cases,
        actions,
        action_combinations,
        load_combinations,
        result_combinations,
        design_situations,
        combination_wizards,
        stability_analysis_settings,
        static_analysis_settings,
        modal_analysis_settings,
        coordinate_systems,
        object_snaps,
        clipping_planes,
        clipping_boxes,
        dimensions,
        line_grids,
        notes,
        surface_reinforcements,
        concrete_durabilities,
        blocks,
        global_parameters,
        soil_samples
    ];
    if (PRERELEASE_MODE)
    {
        containers.push(cutting_line_settings);
        containers.push(cutting_patterns);
        containers.push(harmonic_response_analysis_settings);
        containers.push(enlarged_column_heads);
        containers.push(punching_reinforcements);
    }
}

if (RSTAB) {
    var containers = [
        nodes,
        materials,
        nodal_supports,
        sections,
        member_hinges,
        member_eccentricities,
        member_result_intermediate_points,
        members,
        member_supports,
        member_nonlinearities,
        member_sets,
        member_stiffness_modifications,
        structure_modifications,
        member_definable_stiffnesses,
        design_supports,
        member_loads_from_area_load,
        member_loads_from_free_line_load,
        object_selections,
        snow_loads,
        wind_loads,
        timber_joints,
        imperfection_cases,
        structure_modifications,
        load_cases,
        actions,
        action_combinations,
        load_combinations,
        result_combinations,
        design_situations,
        combination_wizards,
        stability_analysis_settings,
        static_analysis_settings,
        modal_analysis_settings,
        coordinate_systems,
        object_snaps,
        clipping_planes,
        clipping_boxes,
        dimensions,
        line_grids,
        notes,
        concrete_durabilities,
        blocks,
        global_parameters
    ];
    if (PRERELEASE_MODE)
    {
        containers.push(harmonic_response_analysis_settings);
    }
}

if (RSECTION) {
    var containers = [
        materials,
        points,
        lines,
        parts,
        openings,
        elements,
        stress_points,
        load_cases,
        load_combinations
    ];
}

for (var i = 0; i < containers.length; ++i) {
    var container = containers[i];
    for (var j = container.count(); j > 0; --j) {
        try {
            container.erase(container.getNthObjectId(j));
        }
        catch (err){
        }
    }
}

applyChanges();
