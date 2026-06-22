using Munchausen.Compilation;

namespace Munchausen.Explain;

/// <summary>
/// Builds an <see cref="InferenceReport"/> by walking a frozen plan (a pure read),
/// generating nothing. Nested members appear as a single entry; recursive child
/// types are not expanded.
/// </summary>
internal static class InferenceReportBuilder
{
    public static InferenceReport Build(GenerationPlan plan)
    {
        TypePlan root = plan.Root;
        var members = new List<MemberInferenceReport>(root.Members.Length + root.Derivations.Length);

        foreach (MemberPlan member in root.Members)
        {
            MemberReportData report = member.Report;
            members.Add(new MemberInferenceReport(
                member.Member.Name,
                report.Source,
                report.GeneratorName,
                report.Confidence,
                originDefinition: null,
                derivationOrder: null,
                report.OverriddenRules));
        }

        foreach (DerivationPlan derivation in root.Derivations)
        {
            members.Add(new MemberInferenceReport(
                derivation.Member.Name,
                InferenceSource.Derived,
                "Derive",
                confidence: null,
                originDefinition: null,
                derivation.RegistrationIndex,
                Array.Empty<string>()));
        }

        bool isComplete = members.TrueForAll(m => m.Source != InferenceSource.Unsupported);
        return new InferenceReport(
            plan.DefinitionName, root.ModelType, "en", isComplete, members, plan.BuildDiagnostics);
    }
}
