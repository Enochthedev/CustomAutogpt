using System.Collections.Generic;
using Core.InProg.NLP.POS;
using Core.InProg.NLP;
using Core.InProg.NLP.EntityExtraction;
using Core.InProg.NLP.IntentDetection;
using Core.InProg.NLP.Tokenization;
using GemsAi.Core.Ai;

public class NlpPipeline
{
    private readonly IntentDetector _intentDetector;
    private readonly EntityExtractor _entityExtractor;
    private readonly PosTagger _posTagger;

    public NlpPipeline(IntentDetector intentDetector, EntityExtractor extractor, PosTagger posTagger)
    {
        _intentDetector = intentDetector;
        _entityExtractor = extractor;
        _posTagger = posTagger;
    }

    public NlpResult Process(string text)
    {
        var intent = _intentDetector.DetectIntent(text);
        var entities = _entityExtractor.ExtractEntities(text);
        var pos = _posTagger.Predict(text);

        return new NlpResult
        {
            Intent = intent,
            Entities = entities,
            PosTags = pos
        };
    }
}