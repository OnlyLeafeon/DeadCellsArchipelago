"""
Dead Cells - Archipelago World
__init__.py

Main world class. Orchestrates item pool construction, location creation,
region wiring, win condition, and slot data generation.
"""

from typing import Dict, List, Set, Any
from BaseClasses import Tutorial, ItemClassification
from worlds.AutoWorld import World, WebWorld
from .options import DeadCellsOptions
from .items import (
    ITEM_TABLE, BASE_ID as ITEM_BASE_ID,
    DLC_RISE_OF_GIANT, DLC_BAD_SEED, DLC_FATAL_FALLS,
    DLC_QUEEN_AND_SEA, DLC_PURPLE,
    get_items_for_dlcs, get_filler_items, get_trap_items,
    get_progression_items, item_id, PROG, USFL, FILR, TRAP,
)
from .locations import (
    LOCATION_TABLE, BASE_ID as LOC_BASE_ID,
    get_valid_locations, location_id,
)
from .regions import create_regions, REGION_DLC
from .rules import set_rules as apply_location_rules
from .base_classes import DeadCellsItem
from BaseClasses import LocationProgressType

# ─────────────────────────────────────────────────────────────────────────────
# Items that are always base-game weapons (no blueprint required)
# Excluded when include_base_weapons is off
# ─────────────────────────────────────────────────────────────────────────────
BASE_WEAPONS = {
    "QuickSword", "DualDaggers", "StunMace",
    "DualBow", "ThrowingKnife", "LightningWhip", "ThrowingTorch", "Freeze",
    "Shield", "GreedShield",
    "ExtraHeal",
    "FastGrenade", "IceBomb",
    "StandardTurret", "RootTrap",
}

BASE_META = {#"Flask1" in prog
    "Flask2", "Flask3", "Flask4",
    "Money1", "Money2", "Money3", "Money4",
    "RandomBow", "RandomShield", "RandomCC",
    "Recycling1", "Recycling2",
    "ShopRerolls", "PokebombUnlock", "MirrorUnlock", "BackpackUnlock",
}

BASE_PERKS = {
    "P_CDR_Kill", "P_DmgKill", "P_DmgRevenge",
    "P_DeployedDmg", "P_NoMobAround", "P_CDR_Distance",
    "P_CDR_Parry", "P_DmgParry", "P_HealOnKill",
    "P_Yolo", "P_CDR_Crit",
}

BASE_SKINS = {
    "PrisonerGOG", "PrisonerFrench", "PrisonerRetro", "Snowman", "SantaKLOS",
}

BASE_HEADS = {
    "BlackHoleViolet", "VortexHelloDarkness", "BlowTorch",
}

# Cosmetic categories excluded when include_cosmetics is off
COSMETIC_CATEGORIES = {"Skin", "Head"}


# ─────────────────────────────────────────────────────────────────────────────
# Web world (documentation / hints for the AP website)
# ─────────────────────────────────────────────────────────────────────────────
class DeadCellsWebWorld(WebWorld):
    theme = "dirt"
    tutorials = [
        Tutorial(
            tutorial_name="Setup Guide",
            description="A guide to setting up Dead Cells Archipelago.",
            language="English",
            file_name="setup_en.md",
            link="setup/en",
            authors=["You"],
        )
    ]


# ─────────────────────────────────────────────────────────────────────────────
# Main World class
# ─────────────────────────────────────────────────────────────────────────────
class DeadCellsWorld(World):
    """
    Dead Cells randomizer for Archipelago.
    Randomizes weapons, skills, runes, upgrades, skins and aspects
    across biomes, with full DLC support and Boss Cell difficulty scaling.
    """

    game = "Dead Cells"
    options_dataclass = DeadCellsOptions
    options: DeadCellsOptions
    web = DeadCellsWebWorld()

    item_name_to_id = {
        name: ITEM_BASE_ID + data[0]
        for name, data in ITEM_TABLE.items()
    }
    location_name_to_id = {
        name: LOC_BASE_ID + data["id"]
        for name, data in LOCATION_TABLE.items()
    }

    # Populated in generate_early, used throughout
    enabled_dlcs: Set[str] = set()
    cosmetics: set[str] = set()

    # ── Helpers ───────────────────────────────────────────────────────────────

    def _build_enabled_dlcs(self) -> Set[str]:
        """Resolve which DLCs are active based on options."""
        dlcs = set()
        if self.options.dlc_rise_of_the_giant:
            dlcs.add(DLC_RISE_OF_GIANT)
        if self.options.dlc_the_bad_seed:
            dlcs.add(DLC_BAD_SEED)
        if self.options.dlc_fatal_falls:
            dlcs.add(DLC_FATAL_FALLS)
        if self.options.dlc_the_queen_and_the_sea:
            dlcs.add(DLC_QUEEN_AND_SEA)
        if self.options.dlc_return_to_castlevania:
            dlcs.add(DLC_PURPLE)
        return dlcs


    def _item_enabled(self, item_name: str) -> bool:
        data = ITEM_TABLE[item_name]
        _, classification, dlc = data

    # DLC filter
        if dlc and dlc not in self.enabled_dlcs:
            return False

    # Cosmetic filter
        if not self.options.include_cosmetics.value:
            cat = _ITEM_CATEGORY.get(item_name)
            if cat in COSMETIC_CATEGORIES:
                 return False

    # Base weapon filter
        if not self.options.include_base_weapons.value:
            if item_name in BASE_WEAPONS:
                return False
            
    # Base mutation filter
        if not self.options.include_base_mutations.value:
            if item_name in BASE_PERKS:
                return False

        return True

    # ── AP World interface ────────────────────────────────────────────────────

    def generate_early(self) -> None:
        self.enabled_dlcs = self._build_enabled_dlcs()

        if not self.options.include_base_weapons.value:
            for name in BASE_WEAPONS:
                self.multiworld.push_precollected(self.create_item(name))

        if not self.options.include_base_mutations.value:
            for name in BASE_PERKS:
                self.multiworld.push_precollected(self.create_item(name))

    def create_regions(self) -> None:
        """Create all regions and wire transitions."""
        create_regions(self)

    def create_item(self, name: str) -> DeadCellsItem:
        """Create a single AP item by name."""
        data = ITEM_TABLE[name]
        return DeadCellsItem(
            name,
            data[1],  # classification
            self.item_name_to_id[name],
            self.player,
        )

    def create_items(self):
        multiworld = self.multiworld
        player = self.player
        enabled_dlcs = self.enabled_dlcs

    # ─────────────────────────────────────
    # 1. Gather item groups
    # ─────────────────────────────────────
        progression_items = get_progression_items(enabled_dlcs)  # dict{name: (id, class)}
        useful_items = get_items_for_dlcs(enabled_dlcs)          # dict{name: (id, class)}
        filler_items = get_filler_items(enabled_dlcs)            # list[str]
        trap_items = get_trap_items()                            # list[str]

    # Remove progression from useful
        useful_items = [name for name in useful_items if name not in progression_items]

    # ─────────────────────────────────────
    # 2. Count locations
    # ─────────────────────────────────────
        total_locations = len(self.created_locations)

    # ─────────────────────────────────────
    # 3. Build progression pool (FIXED)
    # ─────────────────────────────────────
        itempool = []

    # Each progression item appears ONCE
        progression_list = list(progression_items.keys())

    # Fix Boss Rune count based on BC option
        max_bc = self.options.boss_cells.value

    # Remove any existing boss runes
        progression_list = [i for i in progression_list if i != "ProgBossRune"]

    # Add correct amount
        progression_list += ["ProgBossRune"] * max_bc

    # Add to pool
        itempool += progression_list

    # Calculate remaining slots
        remaining_slots = total_locations - len(itempool)

        if remaining_slots < 0:
            print("WARNING: Too many progression items, trimming...")
            itempool = itempool[:total_locations]
            remaining_slots = 0

    # ─────────────────────────────────────
    # 4. Add useful items (LIMITED)
    # ─────────────────────────────────────
        useful_limit = int(remaining_slots * 0.5)
        useful_list = list(useful_items)

        itempool += useful_list[:useful_limit]
        remaining_slots -= min(useful_limit, len(useful_list))

    # ─────────────────────────────────────
    # 5. Add traps
    # ─────────────────────────────────────
        trap_percentage = self.options.trap_percentage.value
        trap_count = int(total_locations * trap_percentage / 100)

        itempool += trap_items[:trap_count]
        remaining_slots -= min(trap_count, len(trap_items))

    # ─────────────────────────────────────
    # 6. Fill rest with filler
    # ─────────────────────────────────────
        if remaining_slots > 0:
            import random
            random.shuffle(filler_items)

            filler_cycle = (
                filler_items * ((remaining_slots // len(filler_items)) + 1)
            )[:remaining_slots]

            itempool += filler_cycle

    # ─────────────────────────────────────
    # 7. Safety trim (overflow protection)
    # ─────────────────────────────────────
        if len(itempool) > total_locations:
            overflow = len(itempool) - total_locations
            print(f"WARNING: Trimming {overflow} excess items")

            for _ in range(overflow):
                for i in range(len(itempool) - 1, -1, -1):
                    if itempool[i] in filler_items:
                        itempool.pop(i)
                        break
                else:
                    for i in range(len(itempool) - 1, -1, -1):
                        if itempool[i] in useful_items:
                            itempool.pop(i)
                            break

    # ─────────────────────────────────────
    # 8. Convert to AP items
    # ─────────────────────────────────────
        final_items = []

        for name in itempool:
            item_data = ITEM_TABLE[name]
            item = DeadCellsItem(
                name,
                item_data[1],  # classification
                item_id(name),
                player,
            )
            final_items.append(item)

        multiworld.itempool += final_items

    def set_rules(self) -> None:
        """
        Additional rules beyond region entrances.
        Region-level rules are already set in create_regions().
        Here we apply location-level rules and the completion condition.
        """
        # Location-level rules (blueprints, BSC gates, skin counts, etc.)
        apply_location_rules(self)

        # Victory condition: reach the End region
        self.multiworld.completion_condition[self.player] = \
            lambda state: state.can_reach("End", "Region", self.player)

    def fill_slot_data(self) -> Dict[str, Any]:
        """
        Return slot data sent to the game client on connection.
        The Dead Cells mod uses this to configure the session.
        """
        multiworld = self.multiworld
        player = self.player

        early_locations = [
            loc for loc in multiworld.get_locations(player)
            if getattr(loc, "min_bc", 0) == 0
        ]

        progression_items = [
            item for item in multiworld.itempool
            if item.name in [
            "LadderKey",
            "WallJumpKey",
            "BreakableGroundKey",
            "HomKey",
            "ProgBossRune"
        ]
    ]  

        for item, loc in zip(progression_items, early_locations):
            if not loc.item:
                loc.place_locked_item(item)

        return {
            "boss_cells":                self.options.boss_cells.value,
            "death_link":                bool(self.options.death_link.value),
            "dlc_rise_of_the_giant":     bool(self.options.dlc_rise_of_the_giant.value),
            "dlc_the_bad_seed":          bool(self.options.dlc_the_bad_seed.value),
            "dlc_fatal_falls":           bool(self.options.dlc_fatal_falls.value),
            "dlc_the_queen_and_the_sea": bool(self.options.dlc_the_queen_and_the_sea.value),
            "dlc_return_to_castlevania": bool(self.options.dlc_return_to_castlevania.value),
            "include_cosmetics":         bool(self.options.include_cosmetics.value),
            "include_base_weapons":      bool(self.options.include_base_weapons.value),
            "trap_percentage":           self.options.trap_percentage.value,
        }

    def get_filler_item_name(self) -> str:
        """
        Called by AP when it needs an extra filler item (e.g. for item links).
        Returns a random filler item name valid for this world's DLC set.
        """
        fillers = list(get_filler_items(self.enabled_dlcs).keys())
        return self.random.choice(fillers)


# ─────────────────────────────────────────────────────────────────────────────
# Category cache — built once at import time from items.json
# Used by _item_enabled() to filter cosmetics
# ─────────────────────────────────────────────────────────────────────────────
import json as _json
import os as _os

_ITEMS_JSON_PATH = _os.path.join(_os.path.dirname(__file__), "items.json")
try:
    with open(_ITEMS_JSON_PATH) as _f:
        _raw = _json.load(_f)
    _ITEM_CATEGORY: Dict[str, str] = {k: v["category"] for k, v in _raw.items()}
except FileNotFoundError:
    _ITEM_CATEGORY = {}
