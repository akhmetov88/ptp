namespace TradingPlatform.Enums
{
    public enum NotifyTemplateAlias
    {
        main,
        accreditation,
        howituse,
        about,
        rules,
        privacypolicy,
        faq,
        /// <summary>
        /// Письмо контрагенту об изменении им инфы и последующей дискредитацией
        /// </summary>
        changeInfoMail,
        documents,
        /// <summary>
        /// Юристу об изменении инфы контрагентом
        /// </summary>
        juridicalInfoLetter,
        /// <summary>
        /// Продавцу о выкупленной ставке
        /// </summary>
        redemptionBet,
        /// <summary>
        /// ТОрги проиграны
        /// </summary>
        loserLetter,
        /// <summary>
        /// Регистрация
        /// </summary>
        registration,
        /// <summary>
        /// Регистрация юридического лица
        /// </summary>
        registration_legal,
        /// <summary>
        /// Юридическое лицо активировано
        /// </summary>
        legal_active,
        /// <summary>
        /// Юридическое лицо не активировано
        /// </summary>
        legal_not_active,
        /// <summary>
        /// Оповещение о создании новых торгов
        /// </summary>
        new_trade,
        /// <summary>
        /// Начались торги
        /// </summary>
        start_trad,
        /// <summary>
        /// Изменение данных
        /// </summary>
        change_data,
        /// <summary>
        /// Сделана ставка (изменяемый объем лота)
        /// </summary>
        rate_variable_volume,
        /// <summary>
        /// Сделана ставка (фиксированный объем лота)
        /// </summary>
        rate_fix_volume,
        /// <summary>
        /// Ставка перебита
        /// </summary>
        bet_slaughtered,
        /// <summary>
        /// Выкупили (изменяемый объем лота)
        /// </summary>
        bought_variable_volume,
        /// <summary>
        /// Выкупили (фиксированный объем лота)
        /// </summary>
        bought_fix_volume,
        /// <summary>
        /// Ставка выиграла (изменяемый объем лота)
        /// </summary>
        won_variable_volume,
        /// <summary>
        /// Ставка выиграла (фиксированный объем лота)
        /// </summary>
        won_fix_volume,
        /// <summary>
        /// Снятие аккредитации (блокировка)
        /// </summary>
        lock_accreditation,
        /// <summary>
        /// Продление торгов
        /// </summary>
        extending_trading,
        /// <summary>
        /// Завершились торги
        /// </summary>
        complete_trad,
        /// <summary>
        /// О специально перебитой ставке (публичный торг)
        /// </summary>
        rebetterMail,
        toLawyerToApprove,
        /// <summary>
        /// Письмо продавцу о заключенных сделках
        /// </summary>
        sellerLetter,

             /// <summary>
             /// Письмо продавцу о модерации торгов
             /// </summary>
        approveTrade,    
        /// <summary>
        ///  Письмо продавцу о премодерации торгов
        ///  </summary>
        preapproveTrade

    }

    public enum NotifyType
    {
        /// <summary>
        /// Продавцу о новом участнике
        /// </summary>
        ToSellerAboutNewBuyerInTrade,
        /// <summary>
        /// Начало торгов
        /// </summary>
        ToAllBuyersAboutStartTrade,
        /// <summary>
        /// Покупателю о ставке
        /// </summary>
        ToBuyerAboutHisBet,
        /// <summary>
        /// добавлен в торги 
        /// </summary>
        ToBuyerAboutSuccessIncludingToTrade,
        /// <summary>
        /// Ставка перебита
        /// </summary>
        AboutRebet,
        /// <summary>
        ///  Торги закончились
        ///</summary>
        AboutTradeEnd,
        /// <summary>
        /// Покинувшему торги
        /// </summary>
        AboutLeaveToLeaver,
        /// <summary>
        /// продавцу о покидании торга
        /// </summary>
        AboutLeaveToSeller,
        /// <summary>
        /// Продление торгов
        /// </summary>
        ToBuyersAboutContonuedTrade,
               /// <summary>
               /// Кто-то специально перебил ставку
               /// </summary>
        AboutSpecialRebet,
                 /// <summary>
                 /// Ставка не принята
                 /// </summary>
        AboutWrongBet


    }

    public enum TradeTypes
    {
        openFixed,
        closeFixed,
        openVar,
        closeVar,
        quotation
    }

    public enum CatalogType
    {
        Root,
        Incotherms,
        Nomenclature,
        GOST,
        ShipmentPoint,
        Country,
        Transport,
        Product,
        caption,
        Other,
        TransporTherm

    }
    
}